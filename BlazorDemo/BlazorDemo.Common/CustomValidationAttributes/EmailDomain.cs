using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.Extensions;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class EmailDomain : ValidationAttribute
    {
        public string AllowedDomain { get; set; }

        public EmailDomain() { }

        public EmailDomain(string allowedDomain)
        {
            AllowedDomain = allowedDomain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            var strings = value?.ToString().Split('@');
            return value == null || strings.Length > 1 && strings[1].EqualsInvariantIgnoreCase(AllowedDomain)
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage ?? $"Domain must be {AllowedDomain}", new[] { vc.MemberName });
        }
    }
}
