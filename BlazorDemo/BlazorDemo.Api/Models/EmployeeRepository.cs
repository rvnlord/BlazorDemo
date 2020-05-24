using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Api.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _db;

        public EmployeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _db.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string name, Gender? gender)
        {
            IQueryable<Employee> query = _db.Employees;

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.FirstName.ToLower().Contains(name.ToLower()) || e.LastName.ToLower().Contains(name.ToLower()));
            if (gender != null)
                query = query.Where(e => e.Gender == gender);

            return await query.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _db.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);
        }
        
        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            return await _db.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employeeToAdd)
        {
            employeeToAdd.Id = 0;
            var dbAddedEmployeeEntityEntry = await _db.Employees.AddAsync(employeeToAdd);
            await _db.SaveChangesAsync();
            return dbAddedEmployeeEntityEntry.Entity;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employeeToUpdate)
        {
            var dbEmployee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == employeeToUpdate.Id);

            if (dbEmployee == null) 
                return null;

            dbEmployee.FirstName = employeeToUpdate.FirstName;
            dbEmployee.LastName = employeeToUpdate.LastName;
            dbEmployee.Email = employeeToUpdate.Email;
            dbEmployee.DateOfBirth = employeeToUpdate.DateOfBirth;
            dbEmployee.Gender = employeeToUpdate.Gender;
            dbEmployee.DepartmentId = employeeToUpdate.DepartmentId;
            dbEmployee.PhotoPath = employeeToUpdate.PhotoPath;

            await _db.SaveChangesAsync();

            return dbEmployee;

        }

        public async Task<Employee> DeleteEmployeeAsync(int id)
        {
            var dbEmployee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (dbEmployee == null) 
                return null;

            _db.Employees.Remove(dbEmployee);
            await _db.SaveChangesAsync();
            return dbEmployee;
        }
    }
}
