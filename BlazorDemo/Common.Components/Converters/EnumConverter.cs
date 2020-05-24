using System;
using System.ComponentModel;

namespace CommonLibrary.Converters
{
    public static class EnumConverter
    {
        public static string EnumToStringN(this Enum en)
        {
            if (en == null)
                throw new ArgumentNullException(nameof(en));

            return Enum.GetName(en.GetType(), en)?.Replace("_", " ").Trim();
        }

        public static string EnumToString(this Enum en)
        {
            var strEnumN = en.EnumToStringN();
            if (string.IsNullOrWhiteSpace(strEnumN))
                throw new InvalidEnumArgumentException("Enum has no value for the given number");
            return strEnumN;
        }
    }
}
