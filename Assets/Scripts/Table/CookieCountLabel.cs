using TMPro;
using UnityEngine;

namespace CookieClicker.Table
{
    /// <summary>
    /// Shows how full the table is as "count/capacity", for example "3/10".
    /// </summary>
    public class CookieCountLabel : MonoBehaviour
    {
        [SerializeField] private CookieTable table;
        [SerializeField] private TMP_Text label;

        public string Text => label.text;

        private void OnEnable()
        {
            table.Changed += Show;
            Show(table.Count, table.Capacity);
        }

        private void OnDisable()
        {
            table.Changed -= Show;
        }

        private void Show(int count, int capacity)
        {
            label.text = $"{count}/{capacity}";
        }
    }
}
