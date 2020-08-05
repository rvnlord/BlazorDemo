using System;
using System.Collections;
using System.Collections.Generic;

namespace BlazorDemo.Common.Extensions.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue VorN<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            var val = default(TValue);
            dictionary?.TryGetValue(key, out val);
            return val;
        }

        public static Dictionary<TKey, TValue> ReplaceAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            dict.Clear();
            foreach (var (key, value) in newDict)
                dict[key] = value;

            return dict;
        }

        public static IDictionary ReplaceAll(this IDictionary dict, IDictionary newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            dict.Clear();
            foreach (var (key, value) in (Dictionary<object, object>) newDict)
                dict[key] = value;

            return dict;
        }

        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            foreach (var (key, value) in newDict)
                dict[key] = value;

            return dict;
        }
    }
}
