namespace Modbus.Unme.Common
{
    using System.Globalization;

    internal static class StringUtility
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string DoubleQuote(this string str)
        {
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { str });
        }

        public static string SingleQuote(this string str)
        {
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { str });
        }
    }
}
