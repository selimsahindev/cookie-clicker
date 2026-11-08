using System;
using System.Globalization;
using UnityEngine;

namespace CookieClicker.Upgrades
{
    /// <summary>
    /// Balancing data for one upgrade: how much it costs and what value it gives at each level.
    /// Levels start at 1, which is the game's base value.
    /// </summary>
    [CreateAssetMenu(menuName = "Cookie Clicker/Upgrade Definition", fileName = "Upgrade")]
    public class UpgradeDefinition : ScriptableObject
    {
        public const int StartingLevel = 1;

        public enum ValueGrowth
        {
            Add,
            Multiply
        }

        [SerializeField] private string displayName = "Upgrade";
        [SerializeField] private Sprite icon;

        [Header("Cost: baseCost × costGrowth^(level - 1)")]
        [SerializeField, Min(1)] private long baseCost = 10;
        [SerializeField, Min(1f)] private float costGrowth = 1.15f;
        [Tooltip("0 means no limit.")]
        [SerializeField, Min(0)] private int maxLevel;

        [Header("Value")]
        [SerializeField] private float baseValue = 1f;
        [SerializeField] private ValueGrowth growth = ValueGrowth.Add;
        [Tooltip("Added per level for Add, multiplied per level for Multiply.")]
        [SerializeField] private float valuePerLevel = 1f;
        [Tooltip("C# format string for the value, for example {0:0.00}s or +{0:0%}.")]
        [SerializeField] private string valueFormat = "{0}";

        /// <summary>Stable key used in save files: the asset's file name. Renaming the asset resets its saved level.</summary>
        public string Id => name;
        public string DisplayName => displayName;
        public Sprite Icon => icon;

        public bool IsMaxed(int level)
        {
            return maxLevel > 0 && level >= maxLevel;
        }

        /// <summary>Keeps a level, for example one read from a save, inside the valid range.</summary>
        public int ClampLevel(int level)
        {
            int clamped = Math.Max(StartingLevel, level);
            return maxLevel > 0 ? Math.Min(clamped, maxLevel) : clamped;
        }

        /// <summary>Price of going from <paramref name="level"/> to the next level.</summary>
        public long GetCost(int level)
        {
            int steps = level - StartingLevel;
            return (long)Math.Round(baseCost * Math.Pow(costGrowth, steps));
        }

        public float GetValue(int level)
        {
            int steps = level - StartingLevel;
            if (growth == ValueGrowth.Multiply)
            {
                return baseValue * Mathf.Pow(valuePerLevel, steps);
            }

            return baseValue + valuePerLevel * steps;
        }

        public string FormatValue(int level)
        {
            return string.Format(CultureInfo.InvariantCulture, valueFormat, GetValue(level));
        }

        /// <summary>Used by the editor installer and tests to fill in a new definition.</summary>
        public void Configure(string name, Sprite iconSprite, long cost, float baseVal, ValueGrowth valueGrowth, float perLevel, string format, int levelCap = 0)
        {
            displayName = name;
            icon = iconSprite;
            baseCost = cost;
            baseValue = baseVal;
            growth = valueGrowth;
            valuePerLevel = perLevel;
            valueFormat = format;
            maxLevel = levelCap;
        }
    }
}
