using UnityEngine;

namespace CookieClicker.Shopping
{
    /// <summary>
    /// Balancing data for something bought once in the shop, such as a bigger cookie holder.
    /// </summary>
    [CreateAssetMenu(menuName = "Cookie Clicker/Unlock Definition", fileName = "Unlock")]
    public class UnlockDefinition : ShopItemDefinition
    {
        [Tooltip("Short line under the name, for example \"Holds 15 cookies\".")]
        [SerializeField] private string description = "";

        public string Description => description;

        /// <summary>Used by the editor installer to fill in a new definition.</summary>
        public void Configure(string itemName, Sprite itemIcon, long itemPrice, string itemDescription)
        {
            ConfigureItem(itemName, itemIcon, itemPrice);
            description = itemDescription;
        }
    }
}
