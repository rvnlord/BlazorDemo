using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Backend.EmployeeManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Common.Services.Backend.EmployeeManagement
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _db;

        public DepartmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Department> GetDepartmentAsync(int id)
        {
            return await _db.Departments.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _db.Departments.ToListAsync();
        }
    }
}
