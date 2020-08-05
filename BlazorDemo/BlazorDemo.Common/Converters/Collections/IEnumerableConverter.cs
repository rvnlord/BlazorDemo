using System.Collections.Generic;
using System.Linq;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class IEnumerableConverter
    {
        public static List<KeyValuePair<TKey, TValue>> Flatten<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> deepKvps)
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            foreach (var (key, values) in deepKvps)
                list.AddRange(values.Select(value => new KeyValuePair<TKey, TValue>(key, value)));
            return list;
        }
    }
}
