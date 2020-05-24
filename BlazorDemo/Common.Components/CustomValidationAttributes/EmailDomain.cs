using System;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Extensions;

namespace CommonLibrary.CustomValidationAttributes
{
    public class EmailDomain : ValidationAttribute
    {
        public string AllowedDomain { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strings = value?.ToString().Split('@');
            return value == null || strings.Length > 1 && strings[1].EqualsInvariantIgnoreCase(AllowedDomain)
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage ?? $"Domain must be {AllowedDomain}", new[] { validationContext.MemberName });
        }
    }
}
