using System;
using CookieClicker.Economy;
using UnityEngine;

namespace CookieClicker.Shopping
{
    /// <summary>
    /// Sells one-time unlocks for money from the wallet.
    /// </summary>
    public class Shop : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;

        /// <summary>Raised when the balance changes, so item rows can refresh whether they are affordable.</summary>
        public event Action BalanceChanged;

        private void OnEnable()
        {
            wallet.Changed += HandleBalanceChanged;
        }

        private void OnDisable()
        {
            wallet.Changed -= HandleBalanceChanged;
        }

        public bool CanAfford(long price)
        {
            return wallet.Balance >= price;
        }

        public bool CanBuy(Unlockable unlockable)
        {
            return !unlockable.IsUnlocked && CanAfford(unlockable.Price);
        }

        public bool TryBuy(Unlockable unlockable)
        {
            if (unlockable.IsUnlocked || !wallet.TrySpend(unlockable.Price))
            {
                return false;
            }

            unlockable.Unlock();
            return true;
        }

        private void HandleBalanceChanged(long balance)
        {
            BalanceChanged?.Invoke();
        }
    }
}
