using System;
using CookieClicker.Table;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// Hands cookies from the table to the customer at the front of the queue, one at a time.
    /// </summary>
    public class CookieServer : MonoBehaviour
    {
        [SerializeField] private CookieTable table;
        [SerializeField] private CustomerQueue queue;
        [SerializeField] private Transform exitPoint;
        [SerializeField, Min(0f)] private float serveInterval = 0.35f;

        private float timeSinceLastServe;

        /// <summary>Raised for every cookie handed over. The economy can hook in here later.</summary>
        public event Action<Customer> CookieServed;

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

            if (timeSinceLastServe < serveInterval || !table.TryTakeCookie())
            {
                return;
            }

            timeSinceLastServe = 0f;
            front.ReceiveCookie();
            CookieServed?.Invoke(front);

            if (front.IsSatisfied)
            {
                queue.RemoveFront();
                front.Leave(exitPoint.position);
                CustomerSatisfied?.Invoke(front);
            }
        }
    }
}
