using UnityEngine;

namespace CookieClicker.Shopping.Effects
{
    /// <summary>
    /// What an unlock changes in the game once bought. Add a subclass for each new kind of unlock.
    /// </summary>
    public abstract class UnlockEffect : MonoBehaviour
    {
        public abstract void Apply();
    }
}
