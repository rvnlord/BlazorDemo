using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.CustomValidationAttributes;

namespace BlazorDemo.Models
{
    public class EditEmployeeVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name must be provided")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[ComparePropertyUnlessOtherProvided(nameof(ConfirmEmail))]
        [EmailAddress]
        [EmailDomain(AllowedDomain = "test.com")]
        [DisplayName("Company Email")]
        public string Email { get; set; }

        [ComparePropertyUnlessOtherIsNull(nameof(Email))]
        //[CompareProperty(nameof(Email))]
        [DisplayName("Confirm Company Email")]
        public string ConfirmEmail { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoPath { get; set; }

        [ValidateComplexType]
        public virtual Department Department { get; set; } = new Department();
    }
}
