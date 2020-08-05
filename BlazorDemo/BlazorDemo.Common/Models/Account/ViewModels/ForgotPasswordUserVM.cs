using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class ForgotPasswordUserVM
    {
        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress]
        [EmailDomain("gmail.com")]
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}
