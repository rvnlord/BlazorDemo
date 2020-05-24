using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlazorDemo.Models.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Models.Extensions
{
    public static class JTokenExtensionsExtensions
    {
        public static bool IsNullOrEmpty(this JToken jToken)
        {
            return jToken == null ||
                   jToken.Type == JTokenType.Array && !jToken.HasValues ||
                   jToken.Type == JTokenType.Object && !jToken.HasValues ||
                   jToken.Type == JTokenType.String && string.IsNullOrEmpty(jToken.ToString()) ||
                   jToken.Type == JTokenType.Null;
        }

        public static string K(this JToken jToken) => jToken.To<JProperty>().Name;
        public static JToken V(this JToken jToken) => jToken.To<JProperty>().Value;

        public static JToken RemoveEmptyDescendants(this JToken token)
        {
            if (token == null)
                throw new NullReferenceException(nameof(token));

            if (token.Type == JTokenType.Object)
            {
                var copy = new JObject();
                foreach (var prop in token.Children<JProperty>())
                {
                    var child = prop.Value;
                    if (child.HasValues)
                        child = RemoveEmptyDescendants(child);
                    if (!IsNullOrEmpty(child))
                        copy.Add(prop.Name, child);
                }
                return copy;
            }

            if (token.Type == JTokenType.Array)
            {
                var copy = new JArray();
                foreach (var item in token.Children())
                {
                    var child = item;
                    if (child.HasValues)
                        child = RemoveEmptyDescendants(child);
                    if (!IsNullOrEmpty(child))
                        copy.Add(child);
                }
                return copy;
            }
            return token;
        }
    }
}
