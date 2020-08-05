using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class AdminEditRoleVM
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Role Name is Required")]
        [MinLength(3, ErrorMessage = "Role Name has to be at least 3 characters long")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Role Name should contain only alpha-numeric characters")]
        [RoleNotInUse]
        public string Name { get; set; }

        [DisplayName("User Names")]
        public List<string> UserNames { get; set; } = new List<string>();
    }
}
