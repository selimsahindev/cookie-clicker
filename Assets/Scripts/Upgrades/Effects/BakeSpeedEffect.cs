using CookieClicker.Baking;
using UnityEngine;

namespace CookieClicker.Upgrades.Effects
{
    /// <summary>Value is the number of seconds one cookie takes to bake.</summary>
    public class BakeSpeedEffect : UpgradeEffect
    {
        [SerializeField] private Baker baker;

        public override void Apply(float value)
        {
            baker.BakeDuration = value;
        }
    }
}
