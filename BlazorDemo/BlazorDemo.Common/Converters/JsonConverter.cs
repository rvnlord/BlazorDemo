using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using BlazorDemo.Common.Converters.Collections;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.JsonPatch.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BlazorDemo.Common.Converters
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

    public class DecimalJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteValue($"{(decimal)value:0.########}"); // json deserializer will break hashing because it may deserialize 100 as 1000.0 or 1000.00000000
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return Convert.ToDecimal(reader.Value);
        }
    }

    public class LookupJsonConverter<TKey, TValue> : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsValueType && objectType.IsGenericType)
                return objectType.GetGenericTypeDefinition().In(typeof(ILookup<,>), typeof(Lookup<,>));

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var lookup = (ILookup<TKey, TValue>) value;
            var kvps = lookup.SelectMany(kvp => kvp.ToList().Select(v => new KeyValuePair<TKey, TValue>(kvp.Key, v)));
            
            serializer.Serialize(writer, kvps);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var jLookup = (JArray) JToken.Load(reader);
            var kvps = jLookup.Select(jKvp => new KeyValuePair<TKey, TValue>(
                (TKey) Convert.ChangeType(jKvp["Key"], typeof(TKey)), 
                (TValue) Convert.ChangeType(jKvp["Value"], typeof(TValue)))).ToList();

            return kvps.ToLookup();
        }
    }

    public class ExceptionJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType  == typeof(Exception);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var ex = (Exception) value;
            var exDict = new Dictionary<string, object>();

            var type = ex.GetType();
            exDict["ClassName"] = type.FullName;
            exDict["Message"] = ex.Message;
            exDict["Data"] = ex.Data;
            exDict["InnerException"] = ex.InnerException;
            exDict["HResult"] = ex.HResult;
            exDict["Source"] = ex.Source;

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (!exDict.ContainsKey(p.Name))
                    exDict[p.Name] = p.GetValue(ex);

            serializer.Serialize(writer, exDict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var jException = (JObject) JToken.Load(reader);
            var innerEx = jException["InnerException"].To<Exception>();
            var message = jException["Message"].ToString();
            var className = jException["ClassName"]?.ToString();
            var ex = innerEx != null ? new Exception(message, innerEx) : new Exception(message);

            if (innerEx != null && className != null)
            {
                var type = TypeUtils.GetTypeByName(className);
                var ctor = type.GetConstructor(new[] { typeof(string), typeof(Exception) });
                ex = ctor?.Invoke(new object[] { message, innerEx }) as Exception ?? ex;
            }
            else if (innerEx == null && className != null)
            {
                var type = TypeUtils.GetTypeByName(className);
                var ctor = type.GetConstructor(new[] { typeof(string) });
                ex = ctor?.Invoke(new object[] { message }) as Exception ?? ex;
            }
            
            var data = jException["Data"].To<IDictionary>();
            if (data != null)
                ex.Data.ReplaceAll(data);
            ex.HResult = jException["HResult"].To<int>();
            ex.Source = jException["Source"]?.ToString();

            return ex;
        }
    }

    public static class JsonConverter
    {
        public static JsonSerializer JSerializer()
        {
            var jSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
            };
            jSerializer.Converters.Add(new DecimalJsonConverter());
            jSerializer.Converters.Add(new LookupJsonConverter<string, string>());
            jSerializer.Converters.Add(new ExceptionJsonConverter());
            return jSerializer;
        }

        public static JToken ToJToken(this object o, int depth = 10)
        {
            if (o is string)
                throw new InvalidCastException(nameof(o));

            return JToken.Parse(o.JsonSerialize(depth));
        }

        public static JToken JsonDeserialize(this string str)
        {
            if (!str.IsJson())
                str = $"'{str}'";
            return JToken.Parse(str);
        }

        public static JToken JsonDeserializeOrNull(this string str)
        {
            try
            {
                if (!str.IsJson())
                    str = $"'{str}'";
                return JToken.Parse(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsJson(this string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            json = json.Trim();
            if (json.StartsWithInvariant("{") && json.EndsWithInvariant("}") ||
                json.StartsWithInvariant("[") && json.EndsWithInvariant("]"))
            {
                try
                {
                    JToken.Parse(json);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
            }

            return false;
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

        public static JArray ToJArray(this JToken jToken)
        {
            return (JArray)jToken;
        }

        public static JArray ToJArray(this object o) => o.ToJToken().ToJArray();
        public static JToken ToJToken(this JsonElement je) => je.GetRawText().JsonDeserialize();
        public static JToken AsJsonElementToJToken(this object o) => ((JsonElement) o).GetRawText().JsonDeserialize();
        public static ILookup<string, string> AsJsonElementToStringLookup(this object o) => o.AsJsonElementToJToken().ToJArray().ToLookup(j => j["Key"].ToString(), j => j["Value"].ToString());
    }

}
