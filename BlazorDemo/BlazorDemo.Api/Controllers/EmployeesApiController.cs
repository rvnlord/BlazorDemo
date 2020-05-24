using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Api.Models;
using BlazorDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BlazorDemo.Models.Converters;
using Newtonsoft.Json;

namespace BlazorDemo.Api.Controllers
{
    [Route("api/employees")] // [controller]
    [ApiController]
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesApiController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search(string name, Gender? gender)
        {
            try
            {
                var foundDbEmployees = await _employeeRepository.SearchEmployeesAsync(name, gender);
                if (!foundDbEmployees.Any())
                    return NotFound();

                return Ok(foundDbEmployees.ToJToken());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database ({ex.Message})");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAsync()
        {
            try
            {
                return Ok((await _employeeRepository.GetEmployeesAsync()).ToJToken());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database ({ex.Message})");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployeeAsync(Employee employeeToCreate)
        {
            try
            {
                if (employeeToCreate == null)
                    return BadRequest();

                if (await _employeeRepository.GetEmployeeByEmailAsync(employeeToCreate.Email) != null)
                {
                    ModelState.AddModelError("email", "Employee email already in use");
                    return BadRequest(ModelState);
                }

                var createdEmployee = await _employeeRepository.AddEmployeeAsync(employeeToCreate);
                //return Ok(createdEmployee.ToJToken()); // will work
                //return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = createdEmployee.Id }, createdEmployee.ToJToken()); // won't work
                return CreatedAtRoute(new { id = createdEmployee.Id }, createdEmployee.ToJToken()); // works
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database ({ex.Message})");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var dbEmployee = await _employeeRepository.GetEmployeeByIdAsync(id);
                if (dbEmployee == null)
                    return NotFound();

                return Ok(dbEmployee.ToJToken());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database ({ex.Message})");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Employee>> UpdateEmployeeAsync(Employee employeeToUpdate)
        {
            try
            {
                var updatedDbEmployee = await _employeeRepository.UpdateEmployeeAsync(employeeToUpdate);
                if (updatedDbEmployee == null)
                    return NotFound($"Employee with Id = {employeeToUpdate.Id} not found");

                return Ok(updatedDbEmployee.ToJToken());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Employee>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var deletedDbEmployee = await _employeeRepository.DeleteEmployeeAsync(id);
                if (deletedDbEmployee == null)
                    return NotFound($"Employee with Id = {id} not found");

                return Ok(deletedDbEmployee.ToJToken());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }
    }
}
