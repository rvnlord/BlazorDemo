using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Common.Services.Frontend.EmployeeManagement
{
    public class DepartmentService : IDepartmentService
    {
        readonly HttpClient _httpClient;

        public DepartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _httpClient.GetJsonAsync<List<Department>>("api/departments");
        }
    }
}
