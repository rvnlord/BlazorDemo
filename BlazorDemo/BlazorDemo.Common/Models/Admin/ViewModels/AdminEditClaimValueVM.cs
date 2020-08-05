using System.Collections.Generic;
using System.ComponentModel;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class AdminEditClaimValueVM
    {
        public string Value { get; set; }

        [DisplayName("User Names")]
        public List<string> UserNames { get; set; } = new List<string>();
    }
}
