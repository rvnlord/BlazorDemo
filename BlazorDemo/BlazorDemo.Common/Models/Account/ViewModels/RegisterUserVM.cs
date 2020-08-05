using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class RegisterUserVM
    {
        public Guid? Id { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        [MinLength(3, ErrorMessage = "User Name has to be at least 3 characters long")]
        [UserNameNotInUse]
        [UserManagerCompliant]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress]
        [EmailDomain("gmail.com")]
        [EmailNotInUse]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [UserManagerCompliant]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is Required")]
        [ComparePropertyUnlessOtherIsNull("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
        public string Ticket { get; set; }
    }
}
