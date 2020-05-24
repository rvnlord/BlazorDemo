using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CommonLibrary.CustomValidationAttributes;
using CommonLibrary.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CommonLibrary.Components
{
    public class CustomDataAnnotationsValidator : ComponentBase
    {
        private ValidationMessageStore _messageStore;

        [CascadingParameter] 
        public EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
                throw new InvalidOperationException($"{nameof(DataAnnotationsValidator)} requires a cascading parameter of type {nameof(EditContext)}");

            CurrentEditContext.OnFieldChanged += CurrentEditContext_FieldChanged;
            CurrentEditContext.OnValidationRequested += CurrentEditContext_ValidationRequested;
            _messageStore = new ValidationMessageStore(CurrentEditContext); // reassigning this on every FieldChange would cause duplicate messages 
        }

        private void CurrentEditContext_ValidationRequested(object sender, ValidationRequestedEventArgs e) => ValidateModel();
        private void CurrentEditContext_FieldChanged(object sender, FieldChangedEventArgs e) => ValidateField(e.FieldIdentifier);

        private void ValidateField(FieldIdentifier fieldIdentifier)
        {
            static ComparePropertyUnlessOtherIsNull getCompareAttrOrNUll(ICustomAttributeProvider pi)
                => pi.GetCustomAttributes(true).SingleOrDefault(a => a is ComparePropertyUnlessOtherIsNull) as ComparePropertyUnlessOtherIsNull;

            var model = fieldIdentifier.Model;
            var allProperties = model.GetType().GetProperties();
            var previousValidationMessages = allProperties.ToDictionary(p => p.Name, p => CurrentEditContext.GetValidationMessages(new FieldIdentifier(model, p.Name)).ToArray());

            _messageStore.Clear();

            foreach (var prop in allProperties)
            {
                var vrs = new List<ValidationResult>();
                var vc = new ValidationContext(model) { MemberName = prop.Name };
                var fi = new FieldIdentifier(model, prop.Name);
                var compareAttr = getCompareAttrOrNUll(prop);

                if (prop.Name == fieldIdentifier.FieldName) // current
                {
                    if (!Validator.TryValidateProperty(prop.GetValue(model), vc, vrs))
                        _messageStore.Add(fi, vrs.Select(r => r.ErrorMessage));
                }
                else if (compareAttr != null && compareAttr.OtherPropertyName == fieldIdentifier.FieldName) // with attribute comparing to current
                {
                    _messageStore.Add(fieldIdentifier, previousValidationMessages[prop.Name].Where(vm => !vm.EqualsInvariant(compareAttr.GetErrorMessage(model, vc))).ToArray());
                    if (!Validator.TryValidateValue(prop.GetValue(model), vc, vrs, new[] { compareAttr }))
                        _messageStore.Add(fi, vrs.Select(r => r.ErrorMessage));
                }
                else // all others
                {
                    _messageStore.Add(fi, previousValidationMessages[prop.Name]);
                }
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        private void ValidateModel()
        {
            var model = CurrentEditContext.Model;
            var vc = new ValidationContext(model);
            var vrs = new List<ValidationResult>();
            
            Validator.TryValidateObject(model, vc, vrs, true);

            _messageStore.Clear();
            foreach (var vr in vrs)
            {
                if (!vr.MemberNames.Any())
                {
                    _messageStore.Add(new FieldIdentifier(model, string.Empty), vr.ErrorMessage);
                    continue;
                }

                foreach (var memberName in vr.MemberNames)
                    _messageStore.Add(CurrentEditContext.Field(memberName), vr.ErrorMessage);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
