using System;
using System.ComponentModel;
using BlazorDemo.Common.Extensions;

namespace BlazorDemo.Common.Converters
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

        public static T ToEnum<T>(this object value) where T : struct
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enum");
            return (T)Enum.Parse(typeof(T), value.ToString().RemoveMany(" ", "-"), true);
        }

        public static T? ToEnumN<T>(this object value) where T : struct
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enum");
                return (T)Enum.Parse(typeof(T), value.ToString().RemoveMany(" ", "-"), true);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
