using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Services
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
