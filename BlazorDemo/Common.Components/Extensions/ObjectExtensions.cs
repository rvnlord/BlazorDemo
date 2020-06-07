using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CommonLibrary.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetDisplayName(this object model, string propName)
        {
            var type = model.GetType();
            return ((DisplayNameAttribute)type.GetProperty(propName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName
                   ?? ((type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault() as MetadataTypeAttribute)?
                       .MetadataClassType.GetProperty(propName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                       .SingleOrDefault() as DisplayNameAttribute)?.DisplayName 
                   ?? propName;
        }

        public static FieldInfo GetEventField(this object o, string eventName)
        {
            var type = o.GetType();
            FieldInfo field = null;
            while (type != null)
            {
                // Find events defined as field 
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                // Find events defined as property { add; remove; }
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }
    }
}
