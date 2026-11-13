using CookieClicker.Common;
using TMPro;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// Bubble above a customer showing a cookie icon and how many cookies they got so far ("2/5").
    /// </summary>
    public class SpeechBubble : MonoBehaviour
    {
        [SerializeField] private TMP_Text countLabel;
        [SerializeField] private PopAnimation pop;

        public void SetCount(int received, int wanted)
        {
            countLabel.text = $"{received}/{wanted}";
        }

        public void PlayPop()
        {
            if (pop != null)
            {
                pop.Play();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            PlayPop();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
