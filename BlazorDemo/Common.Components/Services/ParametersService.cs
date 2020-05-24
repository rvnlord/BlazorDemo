using System.Collections.Generic;

namespace CommonLibrary.Services
{
    public class ParametersService : IParametersService
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
