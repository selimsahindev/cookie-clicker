using System;
using CookieClicker.Table;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// Hands cookies from the table to the customer at the front of the queue.
    /// The customer takes as many as they still need in one go; if the table runs short,
    /// they take what is there and pick up the rest as new cookies come out.
    /// </summary>
    public class CookieServer : MonoBehaviour
    {
        [SerializeField] private CookieTable table;
        [SerializeField] private CustomerQueue queue;
        [SerializeField] private Transform exitPoint;
        [SerializeField, Min(0f)] private float serveInterval = 0.35f;

        private float timeSinceLastServe;

        /// <summary>Raised with (customer, cookie count) for every hand-over.</summary>
        public event Action<Customer, int> CookiesServed;

        /// <summary>Raised when a customer has everything they wanted and starts walking away.</summary>
        public event Action<Customer> CustomerSatisfied;

        public float ServeInterval
        {
            get
            {
                return serveInterval;
            }
            set
            {
                serveInterval = Mathf.Max(0f, value);
            }
        }

        private void Update()
        {
            timeSinceLastServe += Time.deltaTime;

            Customer front = queue.Front;
            if (front == null || !front.IsReadyToBeServed)
            {
                return;
            }

            if (timeSinceLastServe < serveInterval)
            {
                return;
            }

            int served = table.TakeCookies(front.CookiesRemaining);
            if (served == 0)
            {
                return;
            }

            timeSinceLastServe = 0f;
            front.ReceiveCookies(served);
            CookiesServed?.Invoke(front, served);

            if (front.IsSatisfied)
            {
                queue.RemoveFront();
                front.Leave(exitPoint.position);
                CustomerSatisfied?.Invoke(front);
            }
        }
    }
}
