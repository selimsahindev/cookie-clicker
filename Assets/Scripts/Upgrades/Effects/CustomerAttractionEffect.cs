using CookieClicker.Customers;
using UnityEngine;

namespace CookieClicker.Upgrades.Effects
{
    /// <summary>Value is the average number of seconds between two new customers.</summary>
    public class CustomerAttractionEffect : UpgradeEffect
    {
        [SerializeField] private CustomerSpawner spawner;
        [SerializeField, Range(0f, 1f)] private float spread = 0.4f;

        public override void Apply(float value)
        {
            spawner.SpawnIntervalRange = new Vector2(value * (1f - spread), value * (1f + spread));
        }
    }
}
