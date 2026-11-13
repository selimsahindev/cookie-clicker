using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookieClicker.UI
{
    /// <summary>
    /// The look of one item card in the shop: picture, title, a detail line and a price button.
    /// Holds no game logic; an offer component on the same card fills it in.
    /// </summary>
    public class ShopRowView : MonoBehaviour
    {
        private const float DimmedAlpha = 0.45f;

        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text detailLabel;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text priceLabel;
        [SerializeField] private CanvasGroup priceGroup;
        [SerializeField] private GameObject lockIcon;

        public Button Button => button;

        public void Show(Sprite picture, string title, string detail)
        {
            icon.sprite = picture;
            titleLabel.text = title;
            detailLabel.text = detail;
        }

        /// <summary>Sets the price button: its text, whether it can be pressed and whether it shows a lock.</summary>
        public void ShowPrice(string text, bool interactable, bool locked)
        {
            priceLabel.text = text;
            button.interactable = interactable;
            priceGroup.alpha = interactable ? 1f : DimmedAlpha;
            lockIcon.SetActive(locked);
        }
    }
}
