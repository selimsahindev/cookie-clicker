using System;
using CookieClicker.Common;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// A customer who walks to the queue, collects the cookies they want and then walks away.
    /// </summary>
    [RequireComponent(typeof(CharacterWalker))]
    public class Customer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpeechBubble bubble;

        private CharacterWalker walker;
        private bool isLeaving;

        public event Action<Customer> Left;

        public int CookiesWanted { get; private set; }
        public int CookiesReceived { get; private set; }
        public bool IsSatisfied => CookiesReceived >= CookiesWanted;
        public bool IsReadyToBeServed => !isLeaving && !IsSatisfied && walker.IsAtTarget;

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

        public void WalkTo(Vector3 destination)
        {
            walker.WalkTo(destination);
        }

        public void ReceiveCookie()
        {
            if (IsSatisfied)
            {
                return;
            }

            CookiesReceived++;
            bubble.SetCount(CookiesReceived, CookiesWanted);
            bubble.PlayPop();
        }

        public void Leave(Vector3 exitPoint)
        {
            isLeaving = true;
            bubble.Hide();
            walker.WalkTo(exitPoint);
        }

        private void Update()
        {
            if (isLeaving && walker.IsAtTarget)
            {
                Left?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
