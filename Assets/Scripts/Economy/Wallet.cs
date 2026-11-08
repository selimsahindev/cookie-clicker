using System;
using CookieClicker.Saving;
using UnityEngine;

namespace CookieClicker.Economy
{
    /// <summary>
    /// The player's money. Upgrades will spend from here later.
    /// </summary>
    public class Wallet : MonoBehaviour, ISaveable
    {
        [SerializeField, Min(0)] private long startingBalance;

        private long balance;

        /// <summary>Raised with the new balance whenever it changes.</summary>
        public event Action<long> Changed;

        public long Balance => balance;

        private void Awake()
        {
            balance = startingBalance;
        }

        public void Add(long amount)
        {
            if (amount <= 0)
            {
                return;
            }

            balance += amount;
            Changed?.Invoke(balance);
        }

        public void Capture(SaveData data)
        {
            data.money = balance;
        }

        public void Restore(SaveData data)
        {
            balance = Math.Max(0, data.money);
            Changed?.Invoke(balance);
        }

        public bool TrySpend(long amount)
        {
            if (amount <= 0 || amount > balance)
            {
                return false;
            }

            balance -= amount;
            Changed?.Invoke(balance);
            return true;
        }
    }
}
