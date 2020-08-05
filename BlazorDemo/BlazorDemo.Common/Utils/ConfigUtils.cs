using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;

namespace BlazorDemo.Common.Utils
{
    public static class ConfigUtils
    {
        private static readonly SemaphoreSlim _syncConfigFIle = new SemaphoreSlim(1, 1);

        public static string DBCS { get; set; }
        public static string ApiBaseUrl { get; set; } = "http://localhost:4658";
        public static string FrontendBaseUrl { get; set; }

        public static async Task SetAppSettingValueAsync(string key, string value, string appSettingsJsonFilePath = null)
        {
            if (appSettingsJsonFilePath == null)
            {
                var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                var pathBeforeAssembly = System.AppContext.BaseDirectory.BeforeLast(assemblyName);
                appSettingsJsonFilePath = Path.Combine(pathBeforeAssembly, assemblyName, "appsettings.json");
            }

            await _syncConfigFIle.WaitAsync();

            var json = File.ReadAllText(appSettingsJsonFilePath);
            var jConfig = json.JsonDeserialize();
            key = key.Replace(":", ".");
            var keysButLast = key.BeforeLastOrNull(".");
            var lastKey = key.AfterLastOrWhole(".");
            var jParent = keysButLast != null ? jConfig.SelectToken(keysButLast) : jConfig;
            jParent[lastKey] = value;
            var modifiedJson = jConfig.JsonSerialize();

            File.WriteAllText(appSettingsJsonFilePath, modifiedJson);

            _syncConfigFIle.Release();
        }
    }
}
