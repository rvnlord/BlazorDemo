using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Common.Services.Frontend.EmployeeManagement
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
            try
            {
                return await _httpClient.GetJsonAsync<Employee>($"api/employees/{id}");
            }
            catch (Exception)
            {
                return new Employee
                {
                    DepartmentId = 1,
                    DateOfBirth = DateTime.Now, 
                    PhotoPath = "images/nophoto.jpg"
                };
            }
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
