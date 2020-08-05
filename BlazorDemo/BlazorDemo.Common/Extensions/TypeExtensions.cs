using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlazorDemo.Common.Converters.Collections;

namespace BlazorDemo.Common.Extensions
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

        public static Dictionary<string, T> GetConstants<T>(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => new KeyValuePair<string, T>(fi.Name, (T) fi.GetRawConstantValue())).ToDictionary();
        }

        public static IEnumerable<Type> GetImplementingTypes(this Type itype) 
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).Where(t => t.GetInterfaces().Contains(itype));
    }
}