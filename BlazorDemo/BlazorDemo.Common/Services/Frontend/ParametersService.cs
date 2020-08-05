using System.Collections.Generic;

namespace BlazorDemo.Common.Services.Frontend
{
    public class ParametersService : IParametersService
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
