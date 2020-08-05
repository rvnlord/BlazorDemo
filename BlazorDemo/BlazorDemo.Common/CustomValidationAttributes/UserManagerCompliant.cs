using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Services.Common;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Utils;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class UserManagerCompliant : ValidationAttribute
    {
        protected string _internalMessage;

        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            var accountService = (ServiceLocator.ServiceProvider?.GetService(typeof(IAccountService)) ?? typeof(IAccountService).GetImplementingTypes().First().GetMethod("Create")?.Invoke(null, new object[] { new HttpClient { BaseAddress = new Uri(ConfigUtils.ApiBaseUrl) } })) as IAccountService; // fallback to manual init
            if (accountService == null)
            {
                _internalMessage = "Unable to retrieve client service that performs the validation, this should never happen";
                return new ValidationResult(_internalMessage, new[] { vc.MemberName });
            }

            var checkUmComplianceResp = accountService.CheckUserManagerCompliance(vc.MemberName, vc.ObjectInstance.GetDisplayName(vc.MemberName), value?.ToString());
            if (checkUmComplianceResp.IsError || !checkUmComplianceResp.Result)
            {
                _internalMessage = checkUmComplianceResp.Message; // or .Errors.First().Value;
                return new ValidationResult(GetErrorMessage(), new[] { vc.MemberName });
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage() => ErrorMessage ?? _internalMessage;
    }
}
