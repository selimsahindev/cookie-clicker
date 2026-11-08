using UnityEngine;

namespace CookieClicker.Upgrades.Effects
{
    /// <summary>
    /// Pushes an upgrade's current value into one game system. Add a subclass for each new kind of upgrade.
    /// </summary>
    public abstract class UpgradeEffect : MonoBehaviour
    {
        public abstract void Apply(float value);
    }
}
