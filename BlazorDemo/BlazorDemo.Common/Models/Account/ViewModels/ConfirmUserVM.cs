using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class ConfirmUserVM
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Confirmation Code")]
        [Required(ErrorMessage = "Confirmation Code is Required")]
        [Base64]
        public string ConfirmationCode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
