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

        [EmailAddress]
        [EmailDomain(AllowedDomain = "test.com")]
        //[ComparePropertyUnlessOtherProvided(nameof(ConfirmEmail))]
        public string Email { get; set; }

        [ComparePropertyUnlessOtherIsNull(nameof(Email))]
        //[CompareProperty(nameof(Email))]
        [DisplayName("Confirm Email")]
        public string ConfirmEmail { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoPath { get; set; }

        [ValidateComplexType]
        public virtual Department Department { get; set; } = new Department();
    }
}
