using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BlazorDemo.Common.Converters.Collections;

namespace BlazorDemo.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetDisplayName(this object model, string propName) 
            => ((DisplayNameAttribute)model.GetType().GetProperty(propName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .SingleOrDefault())?.DisplayName ?? propName;

        public static string[] GetPropertyNames(this object o) => o.GetType().GetProperties().Select(p => p.Name).ToArray();
    }
}