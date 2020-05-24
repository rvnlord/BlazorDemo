using System;
using System.Linq;

namespace CommonLibrary.Extensions
{
    public static class StringConverter
    {
        public static bool EqualsInvariant(this string s, string os) => string.Equals(s, os, StringComparison.InvariantCulture);
        public static bool EqualsInvariantIgnoreCase(this string s, string os) => string.Equals(s, os, StringComparison.InvariantCultureIgnoreCase);
        public static string Take(this string str, int n) => new string(str?.AsEnumerable().Take(n).ToArray());
        public static string Skip(this string str, int n) => new string(str?.AsEnumerable().Skip(n).ToArray());
        public static string TakeLast(this string str, int n) => new string(str?.AsEnumerable().TakeLast(n).ToArray());
        public static string SkipLast(this string str, int n) => new string(str?.AsEnumerable().SkipLast(n).ToArray());
        public static string SkipWhile(this string str, Func<char, bool> condition) => new string(str?.AsEnumerable().SkipWhile(condition).ToArray());
        public static string TakeWhile(this string str, Func<char, bool> condition) => new string(str?.AsEnumerable().TakeWhile(condition).ToArray());
    }
}
