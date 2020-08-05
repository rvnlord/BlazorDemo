using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Backend.EmployeeManagement.Interfaces;

namespace BlazorDemo.Api.Controllers
{
    [Route("api/departments")]
    [AllowAnonymous]
    public class DepartmentApiController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentApiController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentsAsync()
        {
            try
            {
                return Ok((await _departmentRepository.GetDepartmentsAsync()).ToJToken());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database ({ex.Message})");
            }
        }
    }
}
