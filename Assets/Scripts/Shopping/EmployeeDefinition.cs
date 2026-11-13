using UnityEngine;

namespace CookieClicker.Shopping
{
    /// <summary>
    /// A character that can be hired in the shop: the hiring price and the wage they cost over time.
    /// Hiring itself isn't built yet; the shop only shows them.
    /// </summary>
    [CreateAssetMenu(menuName = "Cookie Clicker/Employee Definition", fileName = "Employee")]
    public class EmployeeDefinition : ShopItemDefinition
    {
        public enum WageUnit
        {
            Minute,
            Hour,
            Day
        }

        [SerializeField] private string role = "Helper";
        [SerializeField, Min(0)] private long wage = 1;
        [SerializeField] private WageUnit wageUnit = WageUnit.Minute;

        public string Role => role;
        public long Wage => wage;
        public WageUnit Unit => wageUnit;

        /// <summary>Short unit label shown after the wage, for example "min".</summary>
        public string UnitLabel
        {
            get
            {
                switch (wageUnit)
                {
                    case WageUnit.Hour:
                        return "h";
                    case WageUnit.Day:
                        return "day";
                    default:
                        return "min";
                }
            }
        }

        /// <summary>Used by the editor installer to fill in a new definition.</summary>
        public void Configure(string itemName, Sprite portrait, long hirePrice, string employeeRole, long wagePerUnit, WageUnit unit)
        {
            ConfigureItem(itemName, portrait, hirePrice);
            role = employeeRole;
            wage = wagePerUnit;
            wageUnit = unit;
        }
    }
}
