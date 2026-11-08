using System;
using CookieClicker.Upgrades.Effects;
using UnityEngine;

namespace CookieClicker.Upgrades
{
    /// <summary>
    /// The runtime state of one upgrade: its level, and pushing the matching value into the game through its effect.
    /// </summary>
    public class Upgrade : MonoBehaviour
    {
        [SerializeField] private UpgradeDefinition definition;
        [SerializeField] private UpgradeEffect effect;

        private int level = UpgradeDefinition.StartingLevel;

        public event Action<Upgrade> Changed;

        public UpgradeDefinition Definition => definition;
        public int Level => level;
        public bool IsMaxed => definition.IsMaxed(level);
        public long Cost => definition.GetCost(level);
        public string ValueText => definition.FormatValue(level);

        private void Start()
        {
            ApplyEffect();
        }

        public void LevelUp()
        {
            if (IsMaxed)
            {
                return;
            }

            level++;
            ApplyEffect();
            Changed?.Invoke(this);
        }

        private void ApplyEffect()
        {
            effect.Apply(definition.GetValue(level));
        }
    }
}
