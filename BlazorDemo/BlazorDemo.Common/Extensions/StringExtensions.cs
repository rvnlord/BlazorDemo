using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BlazorDemo.Common.Extensions.Collections;

namespace BlazorDemo.Common.Extensions
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

        public static bool StartsWithInvariant(this string s, string start)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.StartsWith(start, StringComparison.InvariantCulture);
        }

        public static bool EndsWithInvariant(this string s, string end)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.EndsWith(end, StringComparison.InvariantCulture);
        }

        public static string AfterIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Throw,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string After(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Throw,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrNull(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Null,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrNullIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Null,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrWhole(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Whole,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrWholeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Whole,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterN(this string str, string substring, int occurance = 1) => str.AfterOrNull(substring, occurance);
        public static string AfterIgnoreCaseN(this string str, string substring, int occurance = 1) => str.AfterOrNullIgnoreCase(substring, occurance);

        public static string AfterFirst(this string s, string substring)
        {
            return s.After(substring);
        }

        public static string AfterFirstIgnoreCase(this string s, string substring)
        {
            return s.AfterIgnoreCase(substring);
        }

        public static string AfterFirstOrNull(this string s, string substring)
        {
            return s.AfterOrNull(substring);
        }

        public static string AfterFirstN(this string s, string substring)
        {
            return s.AfterFirstOrNull(substring);
        }

        public static string AfterFirstOrNullIgnoreCase(this string s, string substring)
        {
            return s.AfterOrNullIgnoreCase(substring);
        }

        public static string AfterFirstIgnoreCaseN(this string s, string substring)
        {
            return s.AfterFirstOrNullIgnoreCase(substring);
        }

        public static string AfterFirstOrWhole(this string s, string substring)
        {
            return s.AfterOrWhole(substring);
        }

        public static string AfterFirstOrWholeIgnoreCase(this string s, string substring)
        {
            return s.AfterOrWholeIgnoreCase(substring);
        }

        public static string AfterLast(this string s, string substring)
        {
            return s.After(substring, -1);
        }

        public static string AfterLastIgnoreCase(this string s, string substring)
        {
            return s.AfterIgnoreCase(substring, -1);
        }

        public static string AfterLastOrNull(this string s, string substring)
        {
            return s.AfterOrNull(substring, -1);
        }

        public static string AfterLastOrNullIgnoreCase(this string s, string substring)
        {
            return s.AfterOrNullIgnoreCase(substring, -1);
        }

        public static string AfterLastN(this string s, string substring) => s.AfterLastOrNull(substring);
        public static string AfterLastIgnoreCaseN(this string s, string substring) => s.AfterLastOrNullIgnoreCase(substring);

        public static string AfterLastOrWhole(this string s, string substring)
        {
            return s.AfterOrWhole(substring, -1);
        }

        public static string AfterLastOrWholeIgnoreCase(this string s, string substring)
        {
            return s.AfterOrWholeIgnoreCase(substring, -1);
        }

        public static string Before(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Throw,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Throw,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrNull(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Null,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrNullIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Null,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrWhole(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Whole,
                BeforeAfterInternalCaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrWholeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, BeforeAfterInternalNoValueType.Whole,
                BeforeAfterInternalCaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeN(this string str, string substring, int occurance = 1) => str.BeforeOrNull(substring, occurance);
        public static string BeforeIgnoreCaseN(this string str, string substring, int occurance = 1) => str.BeforeOrNullIgnoreCase(substring, occurance);

        public static string BeforeFirst(this string s, string substring)
        {
            return s.Before(substring);
        }

        public static string BeforeFirstIgnoreCase(this string s, string substring)
        {
            return s.BeforeIgnoreCase(substring);
        }

        public static string BeforeFirstOrNull(this string s, string substring)
        {
            return s.BeforeOrNull(substring);
        }

        public static string BeforeFirstN(this string s, string substring)
        {
            return s.BeforeFirstOrNull(substring);
        }

        public static string BeforeFirstOrNullIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrNullIgnoreCase(substring);
        }

        public static string BeforeFirstIgnoreCaseN(this string s, string substring)
        {
            return s.BeforeFirstOrNullIgnoreCase(substring);
        }

        public static string BeforeFirstOrWhole(this string s, string substring)
        {
            return s.BeforeOrWhole(substring);
        }

        public static string BeforeFirstOrWholeIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrWholeIgnoreCase(substring);
        }

        public static string BeforeLast(this string s, string substring)
        {
            return s.Before(substring, -1);
        }

        public static string BeforeLastIgnoreCase(this string s, string substring)
        {
            return s.BeforeIgnoreCase(substring, -1);
        }

        public static string BeforeLastOrNull(this string s, string substring)
        {
            return s.BeforeOrNull(substring, -1);
        }

        public static string BeforeLastOrNullIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrNullIgnoreCase(substring, -1);
        }

        public static string BeforeLastN(this string s, string substring) => s.BeforeLastOrNull(substring);
        public static string BeforeLastIgnoreCaseN(this string s, string substring) => s.BeforeLastOrNullIgnoreCase(substring);

        public static string BeforeLastOrWhole(this string s, string substring)
        {
            return s.BeforeOrWhole(substring, -1);
        }

        public static string BeforeLastOrWholeIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrWholeIgnoreCase(substring, -1);
        }

        private static string BeforeAfterInternal(this string str, string substring, int occurance, BeforeAfterInternalNoValueType nullValue, BeforeAfterInternalCaseType casing, BeforeAfterInternalForMethodType methodType)
        {
            if (occurance == 0)
                throw new ArgumentOutOfRangeException(nameof(occurance));

            if (str.IsNullOrEmpty())
            {
                return nullValue switch
                {
                    BeforeAfterInternalNoValueType.Null => null,
                    BeforeAfterInternalNoValueType.Whole => str,
                    BeforeAfterInternalNoValueType.Throw => throw new ArgumentNullException(nameof(str)),
                    _ => throw new ArgumentNullException(nameof(str))
                };
            }

            if (substring.IsNullOrEmpty())
            {
                return nullValue switch
                {
                    BeforeAfterInternalNoValueType.Null => null,
                    BeforeAfterInternalNoValueType.Whole => str,
                    BeforeAfterInternalNoValueType.Throw => throw new ArgumentNullException(nameof(substring)),
                    _ => throw new ArgumentNullException(nameof(substring))
                };
            }

            if (casing == BeforeAfterInternalCaseType.IgnoreCase ? !str.ContainsIgnoreCase(substring) : !str.ContainsInvariant(substring))
            {
                return nullValue switch
                {
                    BeforeAfterInternalNoValueType.Null => null,
                    BeforeAfterInternalNoValueType.Whole => str,
                    BeforeAfterInternalNoValueType.Throw => throw new Exception("String doesn't contain substring"),
                    _ => throw new Exception("String doesn't contain substring")
                };
            }

            if (casing == BeforeAfterInternalCaseType.IgnoreCase ? str.EqualsIgnoreCase(substring) : str.EqualsInvariant(substring))
            {
                return nullValue switch
                {
                    BeforeAfterInternalNoValueType.Null => null,
                    BeforeAfterInternalNoValueType.Whole => str,
                    BeforeAfterInternalNoValueType.Throw => throw new Exception("String equals substring"),
                    _ => throw new Exception("String equals substring")
                };
            }
            var split = casing == BeforeAfterInternalCaseType.IgnoreCase ? str.SplitIgnoreCase(substring) : str.Split(substring);
            var separators = str.SeparatorsIgnoreCaseOrNull(substring);

            var result = casing switch
            {
                BeforeAfterInternalCaseType.IgnoreCase => methodType switch
                {
                    BeforeAfterInternalForMethodType.After => split[(occurance < 0 ? ^-occurance : occurance)..].JoinAsString(separators[(occurance < 0 ? ^-(occurance + 1) : occurance)..]),
                    BeforeAfterInternalForMethodType.Before => split[..(occurance < 0 ? ^-occurance : occurance)].JoinAsString(separators[..(occurance < 0 ? ^-(occurance + 1) : occurance)]),
                    _ => throw new ArgumentOutOfRangeException(nameof(methodType))
                },
                BeforeAfterInternalCaseType.MaintainCase => methodType switch
                {
                    BeforeAfterInternalForMethodType.After => split[(occurance < 0 ? ^-occurance : occurance)..].JoinAsString(substring),
                    BeforeAfterInternalForMethodType.Before => split[..(occurance < 0 ? ^-occurance : occurance)].JoinAsString(substring),
                    _ => throw new ArgumentOutOfRangeException(nameof(methodType))
                },
                _ => throw new ArgumentOutOfRangeException(nameof(casing))
            };

            return nullValue switch
            {
                BeforeAfterInternalNoValueType.Null => result.IsNullOrEmpty() ? null : result,
                BeforeAfterInternalNoValueType.Whole => result.IsNullOrEmpty() ? str : result,
                BeforeAfterInternalNoValueType.Throw => result.IsNullOrEmpty() ? throw new NullReferenceException(nameof(result)) : result,
                _ => throw new NullReferenceException(nameof(result))
            };
        }

        private enum BeforeAfterInternalNoValueType
        {
            Null,
            Whole,
            Throw
        }

        private enum BeforeAfterInternalCaseType
        {
            MaintainCase,
            IgnoreCase
        }

        private enum BeforeAfterInternalForMethodType
        {
            Before,
            After
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool ContainsIgnoreCase(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(substring, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string[] SeparatorsIgnoreCaseOrNull(this string s, string substring)
        {
            var matches = Regex.Matches(s, Regex.Escape(substring), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase).Select(m => m.Value).ToArray();
            return matches.Any() ? matches : null;
        }

        public static string[] SplitIgnoreCase(this string s, string separator)
        {
            return Regex.Split(s, Regex.Escape(separator), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string strBetween = "")
        {
            return string.Join(strBetween, enumerable);
        }

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string[] substrings)
        {
            if (substrings == null)
                throw new ArgumentNullException(nameof(substrings));

            var strings = enumerable.Cast<string>().ToArray();

            if (strings.Length == 1)
                return strings[0]; // whatever is the separator we have nothing to separate
            if (strings.Length - 1 != substrings.Length)
                throw new ArgumentOutOfRangeException(nameof(substrings));
            return strings.Select((s, i) => s + (i < substrings.Length ? substrings[i] : "")).Aggregate((s1, s2) => s1 + s2);
        }

        public static bool EqualsIgnoreCase(this string str, string ostr)
        {
            return string.Equals(str, ostr, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsInvariant(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(substring, StringComparison.InvariantCulture);
        }

        public static bool ContainsInvariant(this string s, char c)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(c, StringComparison.InvariantCulture);
        }

        public static string ReplaceInvariant(this string s, string src, string dest)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Replace(src, dest, StringComparison.InvariantCulture);
        }

        public static string Remove(this string str, string substring)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.ReplaceInvariant(substring, "");
        }

        public static string RemoveMany(this string str, params string[] substrings)
        {
            return substrings.Aggregate(str, (current, substring) => current.Remove(substring));
        }

        public static bool EndsWithIgnoreCase(this string str, string tail)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (tail == null)
                throw new ArgumentNullException(nameof(tail));

            return str.ToLowerInvariant().EndsWithInvariant(tail.ToLowerInvariant());
        }

        public static bool StartsWithIgnoreCase(this string str, string head)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (head == null)
                throw new ArgumentNullException(nameof(head));

            return str.ToLowerInvariant().StartsWithInvariant(head.ToLowerInvariant());
        }

        public static string SkipLastWhile(this string str, Func<char, bool> condition)
        {
            return new string(str.AsEnumerable().SkipLastWhile(condition).ToArray());
        }
    }
}
