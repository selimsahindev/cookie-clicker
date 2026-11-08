using CookieClicker.Economy;
using UnityEngine;

namespace CookieClicker.Upgrades.Effects
{
    /// <summary>Value is the money earned per sold cookie.</summary>
    public class CookiePriceEffect : UpgradeEffect
    {
        [SerializeField] private CashRegister cashRegister;

        public override void Apply(float value)
        {
            cashRegister.PricePerCookie = (long)Mathf.Round(value);
        }
    }
}
