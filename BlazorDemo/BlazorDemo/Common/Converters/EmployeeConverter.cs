using AutoMapper;
using BlazorDemo.Models;

namespace BlazorDemo.Common.Converters
{
    public static class EmployeeConverter
    {
        public static EditEmployeeVM ToEditEmployeeVM(this Employee employee)
        {
            return new EditEmployeeVM
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                ConfirmEmail = employee.Email,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                DepartmentId = employee.DepartmentId,
                PhotoPath = employee.PhotoPath,
                Department = employee.Department
            };
        }

        public static EditEmployeeVM ToEditEmployeeVM(this Employee employee, IMapper mapper)
        {
            return mapper.Map(employee, new EditEmployeeVM());
        }

        public static Employee ToEmployee(this EditEmployeeVM editEmployeeVM, IMapper mapper)
        {
            return mapper.Map(editEmployeeVM, new Employee());
        }
    }
}
