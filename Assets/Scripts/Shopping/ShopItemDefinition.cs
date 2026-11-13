using UnityEngine;

namespace CookieClicker.Shopping
{
    /// <summary>
    /// What every item in the shop has: a name, a picture and a one-time price.
    /// </summary>
    public abstract class ShopItemDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Item";
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0)] private long price = 100;

        /// <summary>Stable key used in save files: the asset's file name. Renaming the asset resets what was bought.</summary>
        public string Id => name;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public long Price => price;

        protected void ConfigureItem(string itemName, Sprite itemIcon, long itemPrice)
        {
            displayName = itemName;
            icon = itemIcon;
            price = itemPrice;
        }
    }
}
