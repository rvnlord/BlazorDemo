using System.Collections.Generic;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class KeyValuePairConverter
    {
        public static KeyValuePair<TKey, TValue> ToKvp<TKey, TValue>(this TValue value, TKey key) => new KeyValuePair<TKey, TValue>(key, value);
    }
}
