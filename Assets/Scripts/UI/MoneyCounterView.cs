using CookieClicker.Common;
using CookieClicker.Economy;
using TMPro;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// The money indicator at the top of the screen.
    /// </summary>
    public class MoneyCounterView : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;
        [SerializeField] private TMP_Text amountLabel;
        [SerializeField] private RectTransform icon;
        [SerializeField] private PopAnimation iconPop;

        /// <summary>Where flying money should land.</summary>
        public RectTransform Icon => icon;

        private void OnEnable()
        {
            wallet.Changed += Refresh;
            Refresh(wallet.Balance);
        }

        private void OnDisable()
        {
            wallet.Changed -= Refresh;
        }

        /// <summary>Called when a flying bill reaches the counter.</summary>
        public void PlayCollect()
        {
            iconPop.Play();
        }

        private void Refresh(long balance)
        {
            amountLabel.text = MoneyFormatter.Format(balance);
        }
    }
}
