using System;
using System.Threading.Tasks;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Common.Converters
{
    public static class ApiResponseConverter
    {
        public static ApiResponse<TResult> ToGeneric<TResult>(this ApiResponse response, Func<object, TResult> resultConverter)
        {
            return new ApiResponse<TResult>(response.StatusCode, response.Message, response.ValidationMessages, resultConverter(response.Result), response.ResponseException);
        }

        public static async Task<ApiResponse<TResult>> ToGeneric<TResult>(this Task<ApiResponse> responseTask, Func<object, TResult> resultConverter)
        {
            var response = await responseTask;
            return new ApiResponse<TResult>(response.StatusCode, response.Message, response.ValidationMessages, resultConverter(response.Result), response.ResponseException);
        }

        public static ApiResponse<TResult> ToGeneric<TResult>(this ApiResponse response, Func<JToken, TResult> resultConverter)
        {
            return new ApiResponse<TResult>(response.StatusCode, response.Message, response.ValidationMessages, resultConverter(response.Result.AsJsonElementToJToken()), response.ResponseException);
        }

        public static async Task<ApiResponse<TResult>> ToGeneric<TResult>(this Task<ApiResponse> responseTask, Func<JToken, TResult> resultConverter)
        {
            var response = await responseTask;
            return new ApiResponse<TResult>(response.StatusCode, response.Message, response.ValidationMessages, resultConverter(response.Result.AsJsonElementToJToken()), response.ResponseException);
        }
    }
}
