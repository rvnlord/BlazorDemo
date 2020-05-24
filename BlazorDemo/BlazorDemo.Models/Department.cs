using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorDemo.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public Department()
        {

        }

        public Department(DepartmentType deptType)
        {
            Id = (int) deptType;
            Name = Enum.GetName(typeof(DepartmentType), deptType);
        }
    }

    public enum DepartmentType
    {
        None,
        IT,
        HR,
        Payroll,
        Admin
    }
}
