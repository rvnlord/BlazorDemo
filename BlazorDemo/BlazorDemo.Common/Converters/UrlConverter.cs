using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using BlazorDemo.Common.Converters.Collections;
using BlazorDemo.Common.Extensions;

namespace BlazorDemo.Common.Converters
{
    public static class UrlConverter
    {
        public static Dictionary<string, string> QueryStringToDictionary(this string queryString)
        {
            var qsp = queryString.AfterFirstOrNullIgnoreCase("?");
            return qsp == null ? new Dictionary<string, string>() : HttpUtility.ParseQueryString(qsp).AsEnumerable().ToDictionary();
        }

        public static string ToQueryString(this IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count <= 0) return "";
            var sb = new StringBuilder();
            foreach (var (name, value) in parameters)
                sb.Append($"&{name.ToAddressEncoded()}={value.ToAddressEncoded()}");
            return sb.ToString().Skip(1);
        }

        public static string RemoveQueryString(this string url, string key)
        {
            var urlWoQs = url.BeforeFirstOrWholeIgnoreCase("?");
            var qs = url.QueryStringToDictionary();
            qs.Remove(key);
            return $"{urlWoQs}?{qs.ToQueryString()}";
        }

        public static string ToAddressEncoded(this string str) => Uri.EscapeDataString(str); // Uri.EscapeDataString HttpUtility.UrlPathEncode

        public static string HtmlEncode(this string s)
        {
            var sb = new StringBuilder();
            var bytes = s.UTF8ToByteArray();
            foreach (var b in bytes)
            {
                if (b >= 0x41 && b <=0x5A || b >= 0x61 && b <=0x7A || b >= 0x30 && b <=0x39 || b == '-' || b == '.' || b == '_' || b == '~')
                    sb.Append((char) b);
                else
                    sb.Append($"%{Convert.ToString(b, 16).ToUpper()}");
            }
            return sb.ToString();
        }

        public static string HtmlDecode(this string s)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var curr = s[i].ToString();
                if (s[i] == '%')
                {
                    curr = $"{s[i + 1]}{s[i + 2]}";
                    sb.Append((char) Convert.ToByte(curr, 16));
                    i += 2;
                }
                else
                    sb.Append(curr);
            }

            return sb.ToString();
        }
    }
}
