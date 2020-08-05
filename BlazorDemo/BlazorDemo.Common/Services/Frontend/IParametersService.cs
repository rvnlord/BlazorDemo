using System.Collections.Generic;

namespace BlazorDemo.Common.Services.Frontend
{
    public interface IParametersService
    {
        Dictionary<string, object> Parameters { get; set; }
    }
}
