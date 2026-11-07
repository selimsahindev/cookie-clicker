using System;
using TMPro;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// A dollar bill with a "+n $" label. It pops in near the sale, then slides into the money counter and fades out.
    /// </summary>
    public class MoneyPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text amountLabel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float popDuration = 0.25f;
        [SerializeField] private float holdDuration = 0.2f;
        [SerializeField] private float flyDuration = 0.55f;
        [SerializeField] private float popOvershoot = 1.25f;
        [SerializeField] private float arrivalScale = 0.5f;

        private RectTransform target;
        private Action onArrived;
        private Vector3 startPosition;
        private float elapsed;

        public void Play(long amount, RectTransform flyTarget, Action arrived)
        {
            amountLabel.text = MoneyFormatter.FormatGain(amount);
            target = flyTarget;
            onArrived = arrived;
            startPosition = transform.position;
            elapsed = 0f;
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed < popDuration)
            {
                AnimatePop(elapsed / popDuration);
                return;
            }

            float flyTime = elapsed - popDuration - holdDuration;
            if (flyTime < 0f)
            {
                transform.localScale = Vector3.one;
                return;
            }

            float t = Mathf.Clamp01(flyTime / flyDuration);
            AnimateFly(t);

            if (t >= 1f)
            {
                onArrived?.Invoke();
                Destroy(gameObject);
            }
        }

        private void AnimatePop(float t)
        {
            float scale = t < 0.6f
                ? Mathf.Lerp(0f, popOvershoot, t / 0.6f)
                : Mathf.Lerp(popOvershoot, 1f, (t - 0.6f) / 0.4f);
            transform.localScale = Vector3.one * scale;
        }

        private void AnimateFly(float t)
        {
            float eased = t * t;
            transform.position = Vector3.Lerp(startPosition, target.position, eased);
            transform.localScale = Vector3.one * Mathf.Lerp(1f, arrivalScale, eased);
            canvasGroup.alpha = 1f - Mathf.Clamp01((t - 0.7f) / 0.3f);
        }
    }
}
