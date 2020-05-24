using System;
using BlazorDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Api.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder mb)
        {
            mb.Entity<Department>().HasData(new Department(DepartmentType.IT));
            mb.Entity<Department>().HasData(new Department(DepartmentType.HR));
            mb.Entity<Department>().HasData(new Department(DepartmentType.Payroll));
            mb.Entity<Department>().HasData(new Department(DepartmentType.Admin));

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Hastings",
                Email = "john@test.com",
                DateOfBirth = new DateTime(1980, 10, 5),
                Gender = Gender.Male,
                DepartmentId = 1,
                PhotoPath = "images/john.png"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 2,
                FirstName = "Sam",
                LastName = "Galloway",
                Email = "sam@test.com",
                DateOfBirth = new DateTime(1981, 12, 22),
                Gender = Gender.Male,
                DepartmentId = 2,
                PhotoPath = "images/sam.jpg"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 3,
                FirstName = "Mary",
                LastName = "Smith",
                Email = "mary@test.com",
                DateOfBirth = new DateTime(1979, 11, 11),
                Gender = Gender.Female,
                DepartmentId = 1,
                PhotoPath = "images/mary.png"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 4,
                FirstName = "Sara",
                LastName = "Longway",
                Email = "sara@test.com",
                DateOfBirth = new DateTime(1982, 9, 23),
                Gender = Gender.Female,
                DepartmentId = 3,
                PhotoPath = "images/sara.png"
            });
        }
    }
}
