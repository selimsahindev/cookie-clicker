using System;
using System.Collections.Generic;

namespace CookieClicker.Saving
{
    /// <summary>
    /// Everything that survives closing the game. Plain data so JsonUtility can write it.
    /// Bump <see cref="CurrentVersion"/> when the shape changes in a way old saves can't load.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public long savedAtUtcTicks;
        public long money;
        public int cookiesOnCart;
        public float bakeProgress;
        public long goalEarnings;
        public List<UpgradeLevel> upgrades = new List<UpgradeLevel>();
        public List<string> unlocks = new List<string>();

        [Serializable]
        public class UpgradeLevel
        {
            public string id;
            public int level;
        }

        public int GetUpgradeLevel(string id, int fallback)
        {
            UpgradeLevel saved = upgrades.Find(entry => entry.id == id);
            return saved != null ? saved.level : fallback;
        }

        public void SetUpgradeLevel(string id, int level)
        {
            UpgradeLevel saved = upgrades.Find(entry => entry.id == id);
            if (saved == null)
            {
                upgrades.Add(new UpgradeLevel { id = id, level = level });
                return;
            }

            saved.level = level;
        }

        public bool HasUnlock(string id)
        {
            return unlocks.Contains(id);
        }

        public void AddUnlock(string id)
        {
            if (!HasUnlock(id))
            {
                unlocks.Add(id);
            }
        }
    }
}
