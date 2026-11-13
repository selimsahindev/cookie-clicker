using CookieClicker.Shopping;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// Shows a hireable character on a card: name, role, wage per time unit and hiring price.
    /// Hiring isn't built yet, so the price button stays locked.
    /// </summary>
    public class EmployeeOfferView : MonoBehaviour
    {
        [SerializeField] private EmployeeDefinition employee;
        [SerializeField] private ShopRowView row;

        private void OnEnable()
        {
            string wage = $"{employee.Role} · {MoneyFormatter.Format(employee.Wage)} $ / {employee.UnitLabel}";
            row.Show(employee.Icon, employee.DisplayName, wage);
            row.ShowPrice($"{MoneyFormatter.Format(employee.Price)} $", false, true);
        }
    }
}
