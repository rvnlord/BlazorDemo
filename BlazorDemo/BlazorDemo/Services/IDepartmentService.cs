using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Models;

namespace BlazorDemo.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync();
    }
}
