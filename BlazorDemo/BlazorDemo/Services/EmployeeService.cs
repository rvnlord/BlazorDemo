using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;

        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _httpClient.GetJsonAsync<Employee[]>("api/employees");
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _httpClient.GetJsonAsync<Employee>($"api/employees/{id}");
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employeeToUpdate)
        {
            return await _httpClient.PutJsonAsync<Employee>("api/employees", employeeToUpdate);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employeeToCreate)
        {
            return await _httpClient.PostJsonAsync<Employee>("api/employees", employeeToCreate);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/employees/{id}");
        }
    }
}
