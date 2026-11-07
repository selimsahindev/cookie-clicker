using System.Globalization;

namespace CookieClicker.UI
{
    /// <summary>
    /// Single place that decides how money is written on screen.
    /// </summary>
    public static class MoneyFormatter
    {
        public static string Format(long amount)
        {
            return amount.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static string FormatGain(long amount)
        {
            return $"+{Format(amount)} $";
        }
    }
}
