using CookieClicker.Table;
using UnityEngine;

namespace CookieClicker.Shopping.Effects
{
    /// <summary>Swaps in a bigger cookie holder: the cart can hold more cookies and the holder widens to fit.</summary>
    public class HolderCapacityEffect : UnlockEffect
    {
        [SerializeField] private CookieTable table;
        [SerializeField, Min(1)] private int capacity = 15;

        public override void Apply()
        {
            table.Capacity = Mathf.Max(table.Capacity, capacity);
        }
    }
}
