using UnityEngine;

namespace CookieClicker.Common
{
    /// <summary>
    /// Moves a character toward a target point and bobs its visual while walking.
    /// </summary>
    public class CharacterWalker : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float bobHeight = 0.08f;
        [SerializeField] private float bobFrequency = 10f;

        private Vector3 target;
        private float walkTime;

        public bool IsAtTarget { get; private set; } = true;

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = Mathf.Max(0f, value);
            }
        }

        public void WalkTo(Vector3 destination)
        {
            target = destination;
            IsAtTarget = transform.position == target;
        }

        private void Update()
        {
            if (IsAtTarget)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            walkTime += Time.deltaTime;
            IsAtTarget = transform.position == target;
            UpdateBob();
        }

        private void UpdateBob()
        {
            if (visual == null)
            {
                return;
            }

            float height = IsAtTarget ? 0f : Mathf.Abs(Mathf.Sin(walkTime * bobFrequency)) * bobHeight;
            visual.localPosition = new Vector3(0f, height, 0f);
        }
    }
}
