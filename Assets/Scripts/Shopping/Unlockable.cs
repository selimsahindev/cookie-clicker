using System;
using CookieClicker.Saving;
using CookieClicker.Shopping.Effects;
using UnityEngine;

namespace CookieClicker.Shopping
{
    /// <summary>
    /// The runtime state of one shop unlock: whether it was bought, and applying its effect once it is.
    /// </summary>
    public class Unlockable : MonoBehaviour, ISaveable
    {
        [SerializeField] private UnlockDefinition definition;
        [SerializeField] private UnlockEffect effect;

        private bool isUnlocked;

        public event Action<Unlockable> Changed;

        public UnlockDefinition Definition => definition;
        public bool IsUnlocked => isUnlocked;
        public long Price => definition.Price;

        /// <summary>Unlocks raise limits (like cart capacity), so they load before the things those limits clamp.</summary>
        public int RestoreOrder => -1;

        public void Unlock()
        {
            if (isUnlocked)
            {
                return;
            }

            isUnlocked = true;
            effect.Apply();
            Changed?.Invoke(this);
        }

        public void Capture(SaveData data)
        {
            if (isUnlocked)
            {
                data.AddUnlock(definition.Id);
            }
        }

        public void Restore(SaveData data)
        {
            if (!data.HasUnlock(definition.Id))
            {
                return;
            }

            Unlock();
        }
    }
}
