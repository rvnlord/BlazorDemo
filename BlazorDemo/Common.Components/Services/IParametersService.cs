using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Services
{
    public interface IParametersService
    {
        Dictionary<string, object> Parameters { get; set; }
    }
}
