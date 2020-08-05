using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Common;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Utils;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class ExistingPassword : ValidationAttribute
    {
        private string _internalMessage;

        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            var ido = vc.ObjectType.GetProperty(nameof(EditUserVM.Id))?.GetValue(vc.ObjectInstance);
            if (ido == null)
            {
                _internalMessage = "User Id is null, it should not have happened";
                return new ValidationResult(_internalMessage, new[] { vc.MemberName });
            }

            var id = (Guid) ido;
            var accountService = ServiceLocator.ServiceProvider?.GetService<IAccountService>() ?? new AccountService(new HttpClient { BaseAddress = new Uri(ConfigUtils.ApiBaseUrl) }, null, null) ; // fallback to manual init
            var password = value?.ToString();
            var apiResponse = accountService.VerifyUserPassword(id, password);
            if (apiResponse.IsError)
            {
                _internalMessage = apiResponse.Message;
                return new ValidationResult(_internalMessage, new[] { vc.MemberName });
            }

            if (!apiResponse.Result)
            {
                _internalMessage = "Password doesn't match the existing one";
                return new ValidationResult(GetErrorMessage(), new[] { vc.MemberName });
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage() => ErrorMessage ?? _internalMessage;
    }
}
