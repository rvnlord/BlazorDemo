using System;
using System.Collections.Generic;
using System.Linq;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Admin.ViewModels;

namespace BlazorDemo.Common.Models.Account.ViewModels
{
    public class FindUserVM
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsConfirmed { get; set; }
        public List<FindRoleVM> Roles { get; set; } = new List<FindRoleVM>();
        public List<FindClaimVM> Claims { get; set; } = new List<FindClaimVM>();

        public bool HasRole(string role) => Roles.Any(r => r.Name.EqualsInvariantIgnoreCase(role));
        public bool HasClaim(string claim) => Claims.Any(c => c.Name.EqualsInvariantIgnoreCase(claim));
    }
}
