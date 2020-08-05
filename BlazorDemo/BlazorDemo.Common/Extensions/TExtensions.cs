using System.Collections.Generic;
using System.Linq;

namespace BlazorDemo.Common.Extensions
{
    public static class TExtensions
    {
        public static bool EqualsAny<T>(this T o, params T[] os) => os.Length > 0 && os.Any(s => s.Equals(o));

        public static bool EqualsAny<T>(this T o, IEnumerable<T> os)
        {
            var osArr = os.ToArray();
            return osArr.Length > 0 && osArr.Any(s => s.Equals(o));
        }

        public static bool In<T>(this T o, params T[] os) => o.EqualsAny(os);
        public static bool In<T>(this T o, IEnumerable<T> os) => o.EqualsAny(os);
        public static bool EqAnyIgnoreCase(this string str, params string[] os) => os.Any(s => s.EqualsIgnoreCase(str));
        public static bool EqAnyIgnoreCase(this string str, IEnumerable<string> os) => os.Any(s => s.EqualsIgnoreCase(str));
    }
}
