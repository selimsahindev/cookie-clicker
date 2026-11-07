using System;
using CookieClicker.Customers;
using UnityEngine;

namespace CookieClicker.Economy
{
    /// <summary>
    /// Gets paid for every cookie handed to a customer and puts the money in the wallet.
    /// </summary>
    public class CashRegister : MonoBehaviour
    {
        [SerializeField] private CookieServer server;
        [SerializeField] private Wallet wallet;
        [SerializeField] private Transform payoutPoint;
        [SerializeField, Min(1)] private long pricePerCookie = 1;

        /// <summary>Raised with (amount, world position of the sale) for every cookie sold.</summary>
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
            server.CookieServed += HandleCookieServed;
        }

        private void OnDisable()
        {
            server.CookieServed -= HandleCookieServed;
        }

        private void HandleCookieServed(Customer customer)
        {
            wallet.Add(pricePerCookie);
            Earned?.Invoke(pricePerCookie, payoutPoint.position);
        }
    }
}
