using CookieClicker.Baking;
using UnityEngine;

namespace CookieClicker.Upgrades.Effects
{
    /// <summary>Value is the share of a full bake one tap adds (0.2 = 20%).</summary>
    public class TapPowerEffect : UpgradeEffect
    {
        [SerializeField] private Baker baker;

        public override void Apply(float value)
        {
            baker.ClickBoost = value;
        }
    }
}
