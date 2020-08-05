using System.Globalization;

namespace BlazorDemo.Common.Converters
{
    public static class ByteConverter
    {
        public static string ToStringInvariant(this byte b, string format) => b.ToString(format, CultureInfo.InvariantCulture);
    }
}
