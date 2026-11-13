using System.Collections.Generic;
using UnityEngine;

namespace CookieClicker.Common
{
    /// <summary>
    /// Moves a character through one or more points and bobs its visual while walking.
    /// </summary>
    public class CharacterWalker : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float bobHeight = 0.08f;
        [SerializeField] private float bobFrequency = 10f;

        private readonly Queue<Vector3> waypoints = new Queue<Vector3>();
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
            WalkAlong(destination);
        }

        /// <summary>Walks to each point in order. Replaces any path the character was following.</summary>
        public void WalkAlong(params Vector3[] path)
        {
            waypoints.Clear();
            foreach (Vector3 point in path)
            {
                waypoints.Enqueue(point);
            }

            target = transform.position;
            AdvanceToNextWaypoint();
        }

        private void Update()
        {
            if (IsAtTarget)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            walkTime += Time.deltaTime;
            if (transform.position == target)
            {
                AdvanceToNextWaypoint();
            }

            UpdateBob();
        }

        /// <summary>Skips points the character already stands on, so a path can start at the current position.</summary>
        private void AdvanceToNextWaypoint()
        {
            while (waypoints.Count > 0 && transform.position == target)
            {
                target = waypoints.Dequeue();
            }

            IsAtTarget = transform.position == target;
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
