﻿using System;
using System.Reflection;

namespace CommonLibrary.Extensions
{
    public static class ObjectExtensions
    {
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
