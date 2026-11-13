using CookieClicker.Common;
using CookieClicker.Shopping;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// Shows one shop unlock on a card and buys it when its price button is pressed.
    /// </summary>
    public class UnlockOfferView : MonoBehaviour
    {
        private const string OwnedText = "Owned";

        [SerializeField] private Unlockable unlockable;
        [SerializeField] private Shop shop;
        [SerializeField] private ShopRowView row;
        [SerializeField] private PopAnimation pop;

        private void OnEnable()
        {
            unlockable.Changed += HandleChanged;
            shop.BalanceChanged += Refresh;
            row.Button.onClick.AddListener(Buy);
            Refresh();
        }

        private void OnDisable()
        {
            unlockable.Changed -= HandleChanged;
            shop.BalanceChanged -= Refresh;
            row.Button.onClick.RemoveListener(Buy);
        }

        private void Buy()
        {
            if (shop.TryBuy(unlockable))
            {
                pop.Play();
            }
        }

        private void HandleChanged(Unlockable changed)
        {
            Refresh();
        }

        private void Refresh()
        {
            UnlockDefinition definition = unlockable.Definition;
            row.Show(definition.Icon, definition.DisplayName, definition.Description);

            string price = unlockable.IsUnlocked ? OwnedText : $"{MoneyFormatter.Format(unlockable.Price)} $";
            row.ShowPrice(price, shop.CanBuy(unlockable), false);
        }
    }
}
