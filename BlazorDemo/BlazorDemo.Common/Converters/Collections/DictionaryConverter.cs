using System.Collections.Generic;
using System.Linq;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class DictionaryConverter
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> en) => en.ToDictionary(el => el.Key, el => el.Value);
    }
}
