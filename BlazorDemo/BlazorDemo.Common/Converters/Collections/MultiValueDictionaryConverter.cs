using System.Collections.Generic;
using System.Linq;
using Microsoft.Collections.Extensions;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class MultiValueDictionaryConverter
    {
        public static MultiValueDictionary<TKey, TValue> ToMultiValueDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
        {
            var mvd = new MultiValueDictionary<TKey, TValue>();
            foreach (var (key, value) in kvps)
                mvd.Add(key, value);
            return mvd;
        }

        public static List<KeyValuePair<TKey, TValue>> ToFlatList<TKey, TValue>(this MultiValueDictionary<TKey, TValue> mvd) => mvd.ToList().Flatten();

        public static MultiValueDictionary<TKey, TValue> ToMultiValueDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, List<TValue>>> kvps)
        {
            var mvd = new MultiValueDictionary<TKey, TValue>();
            foreach (var (key, values) in kvps)
                foreach (var value in values)
                    mvd.Add(key, value);
            return mvd;
        }
    }
}
