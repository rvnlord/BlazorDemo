using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace BlazorDemo.Common.Converters.Collections
{
    public static class NameValueCollectionConverter
    {
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this NameValueCollection nvc)
        {
            if (nvc == null)
                throw new ArgumentNullException(nameof(nvc));

            return nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));
        }
    }
}
