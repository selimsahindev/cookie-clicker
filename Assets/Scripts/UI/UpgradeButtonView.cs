using CookieClicker.Common;
using CookieClicker.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookieClicker.UI
{
    /// <summary>
    /// One upgrade card in the bottom bar: icon on top, then level, current value and the price of the next level.
    /// </summary>
    public class UpgradeButtonView : MonoBehaviour
    {
        private const float UnaffordableAlpha = 0.45f;

        [SerializeField] private Upgrade upgrade;
        [SerializeField] private UpgradeShop shop;

        [Header("Parts")]
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text valueLabel;
        [SerializeField] private TMP_Text costLabel;
        [SerializeField] private CanvasGroup costGroup;
        [SerializeField] private PopAnimation pop;

        private void OnEnable()
        {
            upgrade.Changed += HandleUpgradeChanged;
            shop.BalanceChanged += Refresh;
            button.onClick.AddListener(Buy);
            Refresh();
        }

        private void OnDisable()
        {
            upgrade.Changed -= HandleUpgradeChanged;
            shop.BalanceChanged -= Refresh;
            button.onClick.RemoveListener(Buy);
        }

        private void Buy()
        {
            if (shop.TryBuy(upgrade))
            {
                pop.Play();
            }
        }

        private void HandleUpgradeChanged(Upgrade changed)
        {
            Refresh();
        }

        private void Refresh()
        {
            UpgradeDefinition definition = upgrade.Definition;
            icon.sprite = definition.Icon;
            nameLabel.text = definition.DisplayName;
            levelLabel.text = $"Lv {upgrade.Level}";
            valueLabel.text = upgrade.ValueText;
            costLabel.text = upgrade.IsMaxed ? "MAX" : $"{MoneyFormatter.Format(upgrade.Cost)} $";

            bool affordable = shop.CanAfford(upgrade);
            button.interactable = affordable;
            costGroup.alpha = affordable ? 1f : UnaffordableAlpha;
        }
    }
}
