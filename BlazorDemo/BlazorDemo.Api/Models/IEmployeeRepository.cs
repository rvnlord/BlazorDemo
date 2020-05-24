using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Models;

namespace BlazorDemo.Api.Models
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string name, Gender? gender);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> GetEmployeeByEmailAsync(string email);
        Task<Employee> AddEmployeeAsync(Employee employeeToAdd);
        Task<Employee> UpdateEmployeeAsync(Employee employeeToUpdate);
        Task<Employee> DeleteEmployeeAsync(int id);
    }
}
