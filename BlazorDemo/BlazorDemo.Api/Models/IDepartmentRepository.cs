using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Models;

namespace BlazorDemo.Api.Models
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<Department> GetDepartmentAsync(int id);
    }
}
