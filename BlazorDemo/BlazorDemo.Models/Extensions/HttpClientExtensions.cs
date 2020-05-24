using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorDemo.Models.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<T> CustomPostJsonAsync<T>(this HttpClient httpClient, string requestUri, object content)
            => httpClient.CustomSendJsonAsync<T>(HttpMethod.Post, requestUri, content);

        public static async Task<T> CustomSendJsonAsync<T>(this HttpClient httpClient, HttpMethod method, string requestUri, object content)
        {
            var requestJson = JsonSerializer.Serialize(content);
            var response = await httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            });

            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(IgnoreResponse))
            {
                return default;
            }
            else
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(stringContent);
            }
        }

        private class IgnoreResponse { }
    }
}
