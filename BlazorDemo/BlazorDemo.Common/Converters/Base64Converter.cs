using System;
using System.Text;

namespace BlazorDemo.Common.Converters
{
    public static class Base64Converter
    {
        public static byte[] Base64ToByteArray(this string str)
        {
            try
            {
                return Convert.FromBase64String(str);
            }
            catch (FormatException)
            {
                return new byte[]{};
            }
        }

        public static string ToBase64String(this byte[] arr)
        {
            try
            {
                return Convert.ToBase64String(arr);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        public static string UTF8ToBase64(this string utf8str) => utf8str.UTF8ToByteArray().ToBase64String();

        public static string ToBase64SafeUrlString(this byte[] b) => Convert.ToBase64String(b).TrimEnd(new[] { '=' }).Replace('+', '-').Replace('/', '_');
        public static byte[] Base64SafeUrlToByteArray(this string s)
        {
            var result = s.Replace('_', '/').Replace('-', '+');
            switch (s.Length % 4) 
            {
                case 2: result += "==";  
                    break;
                case 3: result += "="; 
                    break;
            }

            return result.Base64ToByteArray();
        }

        public static string UTF8ToBase64SafeUrl(this string utf8str) => utf8str.UTF8ToByteArray().ToBase64SafeUrlString();
        public static string Base64SafeUrlToUTF8(this string base64str) => base64str.Base64SafeUrlToByteArray().ToUTF8String();
        public static string Base64SafeUrlToUTF8OrNull(this string base64str)
        {
            try
            {
                return base64str.Base64SafeUrlToByteArray().ToUTF8String();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsBase64(this string base64) => Convert.TryFromBase64String(base64.PadRight(base64.Length / 4 * 4 + (base64.Length % 4 == 0 ? 0 : 4), '='), new Span<byte>(new byte[base64.Length]), out _);
    }
}
