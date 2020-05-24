using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorDemo.Models.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsIListType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterface(nameof(IList)) != null;
        }

        public static bool IsGenericCollection(this Type type)
        {
            return type.GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public static bool IsIDictionaryType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterface(nameof(IDictionary)) != null;
        }
    }
}
