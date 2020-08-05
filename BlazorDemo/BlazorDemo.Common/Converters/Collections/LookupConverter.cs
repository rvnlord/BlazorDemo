using System.Collections.Generic;
using System.Linq;
using BlazorDemo.Common.Extensions;
using Microsoft.AspNetCore.Identity;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class LookupConverter
    {
        public static ILookup<TKey, TValue> ToLookup<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps) => kvps.ToLookup(k => k.Key, k => k.Value);

        public static ILookup<string, string> ToLookup(this IEnumerable<IdentityError> errors, IEnumerable<string> propertyNamesToMatchPartially)
        {
            return errors.Select(e => new KeyValuePair<string, string>(propertyNamesToMatchPartially.FirstOrDefault(p => e.Code.ContainsIgnoreCase(p)), e.Description)).Where(kvp => kvp.Key != null).ToLookup();
        }

        public static ILookup<string, string> ToSinglePropertyLookup(this IEnumerable<IdentityError> errors, string propertyForAllMessages)
        {
            return errors.Select(e => new KeyValuePair<string, string>(propertyForAllMessages, e.Description)).ToLookup();
        }
    }
}
