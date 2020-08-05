using System.ComponentModel.DataAnnotations;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class Base64 : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            return value?.ToString().IsBase64() == false ? new ValidationResult(GetErrorMessage(vc), new[] { vc.MemberName }) : ValidationResult.Success;
        }

        public string GetErrorMessage(ValidationContext vc) => ErrorMessage ?? $"Base64 Format for \"{vc.ObjectInstance.GetDisplayName(vc.MemberName)}\" is Required";
    }
}
