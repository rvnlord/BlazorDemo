using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.EmployeeManagement;

namespace BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync();
    }
}
