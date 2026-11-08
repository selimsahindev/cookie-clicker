using System;
using CookieClicker.Customers;
using UnityEngine;

namespace CookieClicker.Economy
{
    /// <summary>
    /// Gets paid for the cookies handed to a customer and puts the money in the wallet.
    /// </summary>
    public class CashRegister : MonoBehaviour
    {
        [SerializeField] private CookieServer server;
        [SerializeField] private Wallet wallet;
        [SerializeField] private Transform payoutPoint;
        [SerializeField, Min(1)] private long pricePerCookie = 1;

        /// <summary>Raised with (total amount, world position of the sale) for every sale.</summary>
        public event Action<long, Vector3> Earned;

        /// <summary>Upgrades can raise the price at runtime.</summary>
        public long PricePerCookie
        {
            get
            {
                return pricePerCookie;
            }
            set
            {
                pricePerCookie = Math.Max(1, value);
            }
        }

        private void OnEnable()
        {
            server.CookiesServed += HandleCookiesServed;
        }

        private void OnDisable()
        {
            server.CookiesServed -= HandleCookiesServed;
        }

        private void HandleCookiesServed(Customer customer, int cookieCount)
        {
            long amount = pricePerCookie * cookieCount;
            wallet.Add(amount);
            Earned?.Invoke(amount, payoutPoint.position);
        }
    }
}
