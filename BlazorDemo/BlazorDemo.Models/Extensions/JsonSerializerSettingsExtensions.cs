using Newtonsoft.Json;

namespace BlazorDemo.Models.Extensions
{
    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings SetSettings(this JsonSerializerSettings oldSettings, JsonSerializerSettings newSettings)
        {
            oldSettings.Context = newSettings.Context;
            oldSettings.Culture = newSettings.Culture;
            oldSettings.ContractResolver = newSettings.ContractResolver;
            oldSettings.ConstructorHandling = newSettings.ConstructorHandling;
            oldSettings.CheckAdditionalContent = newSettings.CheckAdditionalContent;
            oldSettings.DateFormatHandling = newSettings.DateFormatHandling;
            oldSettings.DateFormatString = newSettings.DateFormatString;
            oldSettings.DateParseHandling = newSettings.DateParseHandling;
            oldSettings.DateTimeZoneHandling = newSettings.DateTimeZoneHandling;
            oldSettings.DefaultValueHandling = newSettings.DefaultValueHandling;
            oldSettings.EqualityComparer = newSettings.EqualityComparer;
            oldSettings.FloatFormatHandling = newSettings.FloatFormatHandling;
            oldSettings.Formatting = newSettings.Formatting;
            oldSettings.FloatParseHandling = newSettings.FloatParseHandling;
            oldSettings.MaxDepth = newSettings.MaxDepth;
            oldSettings.MetadataPropertyHandling = newSettings.MetadataPropertyHandling;
            oldSettings.MissingMemberHandling = newSettings.MissingMemberHandling;
            oldSettings.NullValueHandling = newSettings.NullValueHandling;
            oldSettings.ObjectCreationHandling = newSettings.ObjectCreationHandling;
            oldSettings.PreserveReferencesHandling = newSettings.PreserveReferencesHandling;
            oldSettings.ReferenceResolverProvider = newSettings.ReferenceResolverProvider;
            oldSettings.ReferenceLoopHandling = newSettings.ReferenceLoopHandling;
            oldSettings.StringEscapeHandling = newSettings.StringEscapeHandling;
            oldSettings.TraceWriter = newSettings.TraceWriter;
            oldSettings.TypeNameHandling = newSettings.TypeNameHandling;
            oldSettings.SerializationBinder = newSettings.SerializationBinder;
            oldSettings.TypeNameAssemblyFormatHandling = newSettings.TypeNameAssemblyFormatHandling;

            foreach (var converter in newSettings.Converters)
                oldSettings.Converters.Add(converter);

            return oldSettings;
        }
    }
}
