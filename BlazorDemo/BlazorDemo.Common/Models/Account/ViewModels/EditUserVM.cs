using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class EditUserVM
    {
        public Guid Id { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        [MinLength(3, ErrorMessage = "User Name has to be at least 3 characters long")]
        [UserNameNotInUse]
        [UserManagerCompliant]
        public string UserName { get; set; }

        [EmailAddress]
        [EmailDomain("gmail.com")]
        [EmailNotInUse]
        public string Email { get; set; }

        [DisplayName("Old Password")]
        [ExistingPassword]
        public string OldPassword { get; set; }

        public bool HasPassword { get; set; }

        [DisplayName("New Password")]
        [UserManagerCompliant]
        public string NewPassword { get; set; }

        [DisplayName("Confirm New Password")]
        [ComparePropertyUnlessOtherIsNull("NewPassword")]
        public string ConfirmNewPassword { get; set; }

        public string ReturnUrl { get; set; }
        public string Ticket { get; set; }
    }
}
