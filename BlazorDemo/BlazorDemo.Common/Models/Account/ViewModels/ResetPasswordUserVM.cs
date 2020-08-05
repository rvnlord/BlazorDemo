using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class ResetPasswordUserVM
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }

        [DisplayName("Reset Password Code")]
        [Base64]
        public string ResetPasswordCode { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [UserManagerCompliant]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is Required")]
        [ComparePropertyUnlessOtherIsNull("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
