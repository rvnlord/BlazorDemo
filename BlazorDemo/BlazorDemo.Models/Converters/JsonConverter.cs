using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BlazorDemo.Models.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BlazorDemo.Models.Converters
{
    public class JsonTextWriterWithDepth : JsonTextWriter
    {
        public JsonTextWriterWithDepth(TextWriter textWriter) : base(textWriter) { }

        public int CurrentDepth { get; private set; }

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }

    public class ConditionalJsonContractResolver : DefaultContractResolver
    {
        private readonly Func<bool> _includeProperty;

        public ConditionalJsonContractResolver(Func<bool> includeProperty)
        {
            _includeProperty = includeProperty;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => _includeProperty() && (shouldSerialize == null || shouldSerialize(obj));
            return property;
        }
    }

    public static class JsonConverter
    {
        public static JsonSerializer JSerializer() => new JsonSerializer
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
        };

        public static JToken ToJToken(this object o, int depth = 10)
        {
            if (o is string)
                throw new InvalidCastException(nameof(o));

            return JToken.Parse(o.JsonSerialize(depth));
        }

        public static string JsonSerialize(this object o, int depth = 10)
        {
            using var strWriter = new StringWriter();
            var jsonWriter = new JsonTextWriterWithDepth(strWriter);
            var jSerializer = JSerializer();
            jSerializer.ContractResolver = new ConditionalJsonContractResolver(() => jsonWriter.CurrentDepth <= depth);
            jSerializer.Serialize(jsonWriter, o);
            jsonWriter.Close();
            return JToken.Parse(strWriter.ToString()).RemoveEmptyDescendants().ToString(Formatting.Indented, jSerializer.Converters.ToArray());
        }

        public static T To<T>(this JToken jToken)
        {
            T o;
            try
            {
                o = !jToken.IsNullOrEmpty()
                    ? jToken.ToObject<T>(JSerializer())
                    : default;
            }
            catch (JsonSerializationException)
            {
                return (T)(object)null;
            }

            if (o == null && typeof(T).IsIListType() && typeof(T).IsGenericType)
            {
                var elT = typeof(T).GetGenericArguments()[0];
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(elT);
                return (T)Activator.CreateInstance(constructedListType);
            }
            if (o == null && typeof(T).IsIDictionaryType() && typeof(T).IsGenericType)
            {
                var keyT = typeof(T).GetGenericArguments()[0];
                var valT = typeof(T).GetGenericArguments()[1];
                var dictType = typeof(Dictionary<,>);
                var constructedDictType = dictType.MakeGenericType(keyT, valT);
                return (T)Activator.CreateInstance(constructedDictType);
            }

            return o;
        }
    }

}
