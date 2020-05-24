using System;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.CustomValidationAttributes;

namespace BlazorDemo.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name must be provided")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        [EmailDomain(AllowedDomain = "test.com", ErrorMessage = "Domain must be 'test.com' (Custom Message)")]
        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoPath { get; set; }

        public virtual Department Department { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Unspecified
    }
}
