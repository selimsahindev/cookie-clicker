using System;
using CookieClicker.Common;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// A customer who walks to the queue, collects the cookies they want, pauses for a moment and then walks away.
    /// Customers who come from the street walk along it, step into the line and go back to the street when done.
    /// </summary>
    [RequireComponent(typeof(CharacterWalker))]
    public class Customer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpeechBubble bubble;
        [Tooltip("Seconds the customer stands still with their cookies before walking away.")]
        [SerializeField, Min(0f)] private float pauseBeforeLeaving = 0.6f;

        private enum State
        {
            Arriving,
            Queueing,
            Pausing,
            Leaving
        }

        private CharacterWalker walker;
        private State state = State.Queueing;
        private float pauseTimeLeft;
        private Vector3 exitPoint;
        private bool cameFromStreet;
        private StreetCrossing crossing;

        /// <summary>Raised when the customer starts walking away, so the line can move up.</summary>
        public event Action<Customer> Departed;

        public event Action<Customer> Left;

        public int CookiesWanted { get; private set; }
        public int CookiesReceived { get; private set; }
        public int CookiesRemaining => Mathf.Max(0, CookiesWanted - CookiesReceived);
        public bool IsSatisfied => CookiesRemaining == 0;
        public bool IsReadyToBeServed => state == State.Queueing && !IsSatisfied && walker.IsAtTarget;

        private void Awake()
        {
            walker = GetComponent<CharacterWalker>();
        }

        public void Initialize(Sprite look, int cookiesWanted)
        {
            body.sprite = look;
            CookiesWanted = Mathf.Max(1, cookiesWanted);
            CookiesReceived = 0;
            bubble.SetCount(CookiesReceived, CookiesWanted);
        }

        /// <summary>
        /// The customer walks along <paramref name="streetCrossing"/> until they reach their spot in line,
        /// and later leaves toward the end of the same crossing. Call before joining the queue.
        /// </summary>
        public void EnterFromStreet(StreetCrossing streetCrossing, float speed)
        {
            crossing = streetCrossing;
            cameFromStreet = true;
            transform.position = crossing.Start;
            walker.Speed = speed;
            bubble.Hide();
            state = State.Arriving;
        }

        public void WalkTo(Vector3 destination)
        {
            if (state == State.Arriving)
            {
                walker.WalkAlong(crossing.PointAt(destination.x), destination);
                return;
            }

            walker.WalkTo(destination);
        }

        public void ReceiveCookies(int amount)
        {
            int accepted = Mathf.Min(amount, CookiesRemaining);
            if (accepted <= 0)
            {
                return;
            }

            CookiesReceived += accepted;
            bubble.SetCount(CookiesReceived, CookiesWanted);
            bubble.PlayPop();
        }

        /// <summary>
        /// Pauses for <see cref="pauseBeforeLeaving"/> seconds, then walks to <paramref name="exit"/>.
        /// Customers from the street ignore <paramref name="exit"/> and keep walking the way they came.
        /// </summary>
        public void Leave(Vector3 exit)
        {
            exitPoint = exit;
            pauseTimeLeft = pauseBeforeLeaving;
            state = State.Pausing;
        }

        private void Update()
        {
            switch (state)
            {
                case State.Arriving:
                    UpdateArrival();
                    break;
                case State.Pausing:
                    UpdatePause();
                    break;
                case State.Leaving:
                    UpdateLeaving();
                    break;
            }
        }

        private void UpdateArrival()
        {
            if (!walker.IsAtTarget)
            {
                return;
            }

            state = State.Queueing;
            bubble.Show();
        }

        private void UpdatePause()
        {
            pauseTimeLeft -= Time.deltaTime;
            if (pauseTimeLeft > 0f)
            {
                return;
            }

            state = State.Leaving;
            bubble.Hide();
            WalkAway();
            Departed?.Invoke(this);
        }

        private void WalkAway()
        {
            if (cameFromStreet)
            {
                walker.WalkAlong(crossing.PointAt(transform.position.x), crossing.End);
                return;
            }

            walker.WalkTo(exitPoint);
        }

        private void UpdateLeaving()
        {
            if (walker.IsAtTarget)
            {
                Left?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
