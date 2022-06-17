using System;

namespace TheLedgerCo.Extensions
{
    public static class StringExtensions
    {
        public static string[] SplitCommand(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return value.Split(" ");
        }

        public static decimal ToDecimal(this string value)
        {
            Decimal.TryParse(value, out decimal number);
            return number;
        }

        public static int ToInt(this string value)
        {
            int.TryParse(value, out int number);
            return number;
        }
    }
}
