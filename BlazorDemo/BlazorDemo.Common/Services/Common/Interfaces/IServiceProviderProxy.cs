using System;
using System.Collections.Generic;

namespace BlazorDemo.Common.Services.Common.Interfaces
{
    public interface IServiceProviderProxy
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }
}
