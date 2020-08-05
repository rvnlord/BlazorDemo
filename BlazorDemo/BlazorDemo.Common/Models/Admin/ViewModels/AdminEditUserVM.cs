using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BlazorDemo.Common.CustomValidationAttributes;
using BlazorDemo.Common.Extensions;
using Microsoft.Collections.Extensions;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class AdminEditUserVM
    {
        public Guid Id { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        [MinLength(3, ErrorMessage = "User Name has to be at least 3 characters long")]
        [UserNameNotInUse]
        public string UserName { get; set; }

        [EmailAddress]
        [EmailNotInUse]
        public string Email { get; set; }

        public string Password { get; set; }

        [DisplayName("Is Deleted")]
        public bool IsDeleted { get; set; }
        [DisplayName("Is Confirmed")]
        public bool IsConfirmed { get; set; }

        public string ReturnUrl { get; set; }
        public List<FindRoleVM> Roles { get; set; } = new List<FindRoleVM>();
        public List<FindClaimVM> Claims { get; set; } = new List<FindClaimVM>();
        public string Ticket { get; set; }

        public bool HasRole(string role) => Roles.Any(r => r.Name.EqualsInvariantIgnoreCase(role));
        public bool HasClaim(string claim) => Claims.Any(c => c.Name.EqualsInvariantIgnoreCase(claim));
    }
}
