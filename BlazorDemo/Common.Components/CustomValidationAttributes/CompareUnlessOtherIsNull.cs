using System;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Extensions;

namespace CommonLibrary.CustomValidationAttributes
{
    public class ComparePropertyUnlessOtherIsNull : ValidationAttribute
    {
        public string OtherPropertyName { get; set; }

        public ComparePropertyUnlessOtherIsNull(string otherPropertyName)
        {
            OtherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);
            if (propInfo == null)
                throw new NullReferenceException("There is no property with specified name");

            var model = validationContext.ObjectInstance;
            var otherProperty = propInfo.GetValue(model);

            return value == null || otherProperty == null || value.Equals(otherProperty) ? ValidationResult.Success : new ValidationResult(GetErrorMessage(model, validationContext));
        }

        public string GetErrorMessage(object model, ValidationContext vc) => ErrorMessage ?? $"'{model.GetDisplayName(vc.MemberName)}' must match '{model.GetDisplayName(OtherPropertyName)}'";
    }
}
