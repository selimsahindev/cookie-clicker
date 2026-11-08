using System;
using CookieClicker.Economy;
using UnityEngine;

namespace CookieClicker.Upgrades
{
    /// <summary>
    /// Sells upgrades for money from the wallet.
    /// </summary>
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;

        /// <summary>Raised when the balance changes, so buttons can refresh whether they are affordable.</summary>
        public event Action BalanceChanged;

        private void OnEnable()
        {
            wallet.Changed += HandleBalanceChanged;
        }

        private void OnDisable()
        {
            wallet.Changed -= HandleBalanceChanged;
        }

        public bool CanAfford(Upgrade upgrade)
        {
            return !upgrade.IsMaxed && wallet.Balance >= upgrade.Cost;
        }

        public bool TryBuy(Upgrade upgrade)
        {
            if (upgrade.IsMaxed || !wallet.TrySpend(upgrade.Cost))
            {
                return false;
            }

            upgrade.LevelUp();
            return true;
        }

        private void HandleBalanceChanged(long balance)
        {
            BalanceChanged?.Invoke();
        }
    }
}
