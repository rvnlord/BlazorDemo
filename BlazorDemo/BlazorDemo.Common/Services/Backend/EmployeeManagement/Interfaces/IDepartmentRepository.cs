using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.EmployeeManagement;

namespace BlazorDemo.Common.Services.Backend.EmployeeManagement.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<Department> GetDepartmentAsync(int id);
    }
}
