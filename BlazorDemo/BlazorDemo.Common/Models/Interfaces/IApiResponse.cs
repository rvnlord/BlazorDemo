using System;
using System.Linq;
using BlazorDemo.Common.Models.Account;

namespace BlazorDemo.Common.Models.Interfaces
{
    public interface IApiResponse
    {
        StatusCodeType StatusCode { get; set; }
        bool IsError { get; }
        string Message { get; set; }
        Exception ResponseException { get; set; }
        object Result { get; set; }
        ILookup<string, string> ValidationMessages { get; set; }
    }
}
