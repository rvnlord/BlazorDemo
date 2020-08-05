using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.EmployeeManagement;

namespace BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<Employee>> GetEmployeesAsync();
        public Task<Employee> GetEmployeeByIdAsync(int id);
        public Task<Employee> UpdateEmployeeAsync(Employee employeeToUpdate);
        public Task<Employee> CreateEmployeeAsync(Employee employeeToCreate);
        public Task DeleteEmployeeAsync(int id);
    }
}
