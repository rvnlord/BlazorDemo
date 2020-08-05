using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BlazorDemo.Common.CustomValidationAttributes;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class AdminEditClaimVM
    {
        public string OriginalName { get; set; } // to validate against since we don't have Id

        [Required(ErrorMessage = "Claim Name is Required")]
        [MinLength(3, ErrorMessage = "Claim Name has to be at least 3 characters long")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Claim Name should contain only alpha-numeric characters")]
        [ClaimNotInUse] // This attribute is also safeguarded in the API methods as an example, other 'NotInUse' Attributes in this project aren't
        public string Name { get; set; }

        public List<AdminEditClaimValueVM> Values { get; set; } = new List<AdminEditClaimValueVM>();

        public List<string> GetUserNames() => Values.SelectMany(cv => cv.UserNames).OrderBy(n => n).ToList();
    }
}
