using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;

namespace BlazorDemo.Common.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<T> PostJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null)
            => httpClient.SendJTokenAsync<T>(HttpMethod.Post, requestUri, content);
        public static Task<T> GetJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null)
            => httpClient.SendJTokenAsync<T>(HttpMethod.Get, requestUri, content);
        public static Task<T> PutJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null)
            => httpClient.SendJTokenAsync<T>(HttpMethod.Put, requestUri, content);
        public static Task<T> DeleteJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null)
            => httpClient.SendJTokenAsync<T>(HttpMethod.Delete, requestUri, content);
        public static Task<T> PatchJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null)
            => httpClient.SendJTokenAsync<T>(HttpMethod.Patch, requestUri, content);

        public static async Task<T> SendJTokenAsync<T>(this HttpClient httpClient, HttpMethod method, string requestUri, object content)
        {
            var requestJson = content.JsonSerialize();
            var response = await httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            });

            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(IgnoreResponse))
                return default;

            var stringContent = await response.Content.ReadAsStringAsync();
            return stringContent.JsonDeserialize().To<T>();
        }

        private class IgnoreResponse { }
    }
}
