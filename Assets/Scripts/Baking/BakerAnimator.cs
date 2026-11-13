using UnityEngine;

namespace CookieClicker.Baking
{
    /// <summary>
    /// Procedural animation for the baker: whisks while baking, rests while the table is full,
    /// gives a small hop when a cookie is done and a light squash on every tap.
    /// Reactions are kept subtle so fast tapping doesn't make the baker jitter.
    /// </summary>
    public class BakerAnimator : MonoBehaviour
    {
        [SerializeField] private Baker baker;
        [SerializeField] private Transform rig;
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;

        [Header("Whisking")]
        [SerializeField] private float whiskSpeed = 14f;
        [SerializeField] private float whiskAngle = 22f;
        [SerializeField] private float bounceHeight = 0.03f;

        [Header("Reactions")]
        [SerializeField] private float hopDuration = 0.25f;
        [SerializeField] private float hopHeight = 0.05f;
        [SerializeField] private float cheerAngle = 35f;
        [SerializeField] private float tapDuration = 0.1f;
        [SerializeField] private float tapSquash = 0.02f;

        private const float LeftArmRestAngle = -10f;
        private const float RightArmRestAngle = 10f;
        private const float LeftArmBakeAngle = -32f;
        private const float RightArmBakeAngle = 28f;

        private Vector3 rigBasePosition;
        private float time;
        private float hopTimeLeft;
        private float tapTimeLeft;

        private void Awake()
        {
            rigBasePosition = rig.localPosition;
        }

        private void OnEnable()
        {
            baker.CookieBaked += PlayHop;
            baker.Boosted += PlayTap;
        }

        private void OnDisable()
        {
            baker.CookieBaked -= PlayHop;
            baker.Boosted -= PlayTap;
        }

        private void PlayHop()
        {
            if (hopTimeLeft > 0f)
            {
                return;
            }

            hopTimeLeft = hopDuration;
        }

        private void PlayTap()
        {
            if (tapTimeLeft > 0f)
            {
                return;
            }

            tapTimeLeft = tapDuration;
        }

        private void Update()
        {
            time += Time.deltaTime;
            hopTimeLeft = Mathf.Max(0f, hopTimeLeft - Time.deltaTime);
            tapTimeLeft = Mathf.Max(0f, tapTimeLeft - Time.deltaTime);

            float hop = Arc(hopTimeLeft, hopDuration);
            float tap = Arc(tapTimeLeft, tapDuration);

            AnimateArms(hop);
            AnimateBody(hop, tap);
        }

        private void AnimateArms(float hop)
        {
            float leftAngle;
            float rightAngle;
            if (baker.IsBaking)
            {
                leftAngle = LeftArmBakeAngle + Mathf.Sin(time * whiskSpeed * 0.5f) * 4f;
                rightAngle = RightArmBakeAngle + Mathf.Sin(time * whiskSpeed) * whiskAngle;
            }
            else
            {
                leftAngle = LeftArmRestAngle;
                rightAngle = RightArmRestAngle;
            }

            leftArm.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(leftAngle, -cheerAngle, hop));
            rightArm.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(rightAngle, cheerAngle, hop));
        }

        private void AnimateBody(float hop, float tap)
        {
            float bounce = baker.IsBaking ? Mathf.Abs(Mathf.Sin(time * whiskSpeed * 0.5f)) * bounceHeight : 0f;
            float breathe = Mathf.Sin(time * 2f) * 0.012f;
            rig.localPosition = rigBasePosition + Vector3.up * (bounce + hop * hopHeight);

            float squash = tap * tapSquash;
            rig.localScale = new Vector3(1f + squash, 1f - squash + breathe, 1f);
        }

        /// <summary>Returns 0 -> 1 -> 0 over a reaction that has <paramref name="timeLeft"/> seconds remaining.</summary>
        private static float Arc(float timeLeft, float duration)
        {
            if (timeLeft <= 0f)
            {
                return 0f;
            }

            return Mathf.Sin((1f - timeLeft / duration) * Mathf.PI);
        }
    }
}
