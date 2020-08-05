using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class LoginUserVM
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
        public string ExternalProvider { get; set; }

        public string ReturnUrl { get; set; }
        public string ExternalProviderKey { get; set; }
        public string Ticket { get; set; }
    }
}
