using System.Collections.Generic;
using System.Linq;

namespace BlazorDemo.Common.Extensions.Collections
{
    public static class LookupExtensions
    {
        public static ILookup<TKey, TValue> RenameKey<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key, TKey newKey)
        {
            return lookup.AddRange(newKey, lookup[key]).Remove(key);
        }

        public static ILookup<TKey, TValue> Remove<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key)
        {
            var kvps = lookup.SelectMany(el => el, (el, v) => new KeyValuePair<TKey, TValue>(el.Key, v)).ToList();
            return kvps.Where(l => !l.Key.Equals(key)).ToLookup(l => l.Key, l => l.Value);
        }

        public static ILookup<TKey, TValue> Add<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key, TValue val)
        {
            var kvps = lookup.SelectMany(el => el, (el, v) => new KeyValuePair<TKey, TValue>(el.Key, v)).ToList();
            return kvps.Append(new KeyValuePair<TKey, TValue>(key, val)).ToLookup(l => l.Key, l => l.Value);
        }

        public static ILookup<TKey, TValue> AddRange<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key, IEnumerable<TValue> vals)
        {
            var kvps = lookup.SelectMany(el => el, (el, v) => new KeyValuePair<TKey, TValue>(el.Key, v)).ToList();
            return kvps.Concat(vals.Select(v => new KeyValuePair<TKey, TValue>(key, v))).ToLookup(l => l.Key, l => l.Value);
        }
    }
}
