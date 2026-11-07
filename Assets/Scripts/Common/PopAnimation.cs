using UnityEngine;

namespace CookieClicker.Common
{
    /// <summary>
    /// Small squash-and-stretch "pop" used to give feedback when something changes.
    /// </summary>
    public class PopAnimation : MonoBehaviour
    {
        [SerializeField] private float duration = 0.18f;
        [SerializeField] private float strength = 0.25f;

        private Vector3 baseScale;
        private float elapsed;

        private void Awake()
        {
            baseScale = transform.localScale;
            elapsed = duration;
        }

        public void Play()
        {
            elapsed = 0f;
        }

        private void Update()
        {
            if (elapsed >= duration)
            {
                return;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * strength;
            transform.localScale = baseScale * scale;
        }
    }
}
