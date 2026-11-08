using System;
using UnityEngine;

namespace CookieClicker.Table
{
    /// <summary>
    /// Holds finished cookies up to a capacity. Bakers add to it, the server takes from it.
    /// </summary>
    public class CookieTable : MonoBehaviour
    {
        [SerializeField, Min(1)] private int capacity = 10;

        private int count;

        /// <summary>Raised with (count, capacity) whenever either value changes.</summary>
        public event Action<int, int> Changed;

        public int Count => count;
        public bool HasSpace => count < capacity;
        public bool HasCookies => count > 0;

        /// <summary>Upgrades can raise this at runtime.</summary>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = Mathf.Max(1, value);
                count = Mathf.Min(count, capacity);
                NotifyChanged();
            }
        }

        public bool TryAddCookie()
        {
            if (!HasSpace)
            {
                return false;
            }

            count++;
            NotifyChanged();
            return true;
        }

        /// <summary>Takes up to <paramref name="amount"/> cookies and returns how many were actually taken.</summary>
        public int TakeCookies(int amount)
        {
            int taken = Mathf.Clamp(amount, 0, count);
            if (taken == 0)
            {
                return 0;
            }

            count -= taken;
            NotifyChanged();
            return taken;
        }

        private void NotifyChanged()
        {
            Changed?.Invoke(count, capacity);
        }
    }
}
