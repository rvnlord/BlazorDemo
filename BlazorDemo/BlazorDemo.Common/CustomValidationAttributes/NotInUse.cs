using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Common;
using BlazorDemo.Common.Services.Frontend;
using BlazorDemo.Common.Utils;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public abstract class NotInUse : ValidationAttribute
    {
        protected string _internalMessage;
        protected Type _clientServiceType;
        protected string _findMethodName;

        protected NotInUse(Type clientServiceType, string findMethodName)
        {
            _clientServiceType = clientServiceType;
            _findMethodName = findMethodName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            var hasId = vc.ObjectType.GetProperty("Id") != null;
            var id = (Guid?) vc.ObjectType.GetProperty("Id")?.GetValue(vc.ObjectInstance);
            if (id == Guid.Empty)
                id = null;
            var hasOrigName = vc.ObjectType.GetProperty("OriginalName") != null;
            var originalName = vc.ObjectType.GetProperty("OriginalName")?.GetValue(vc.ObjectInstance)?.ToString();
            var clientService = ServiceLocator.ServiceProvider?.GetService(_clientServiceType) ?? _clientServiceType.GetImplementingTypes().First().GetMethod("Create")?.Invoke(null, new object[] { new HttpClient { BaseAddress = new Uri(ConfigUtils.ApiBaseUrl) } }); // fallback to manual init
            var roleName = value?.ToString();
            if (roleName.IsNullOrWhiteSpace())
                return ValidationResult.Success;

            var apiResponse = (IApiResponse) _clientServiceType.GetMethod(_findMethodName)?.Invoke(clientService, new object[] { roleName });
            if (apiResponse == null)
            {
                _internalMessage = "Supplied validation method for checking if value is not in use was not found";
                return new ValidationResult(_internalMessage, new[] { vc.MemberName });
            }
            if (apiResponse.IsError && apiResponse.StatusCode != StatusCodeType.Status404NotFound)
            {
                _internalMessage = $"{vc.MemberName} availability cannot be checked, API is offline";
                return new ValidationResult(_internalMessage, new[] { vc.MemberName });
            }
            var result = apiResponse.Result;
            var resultId = result?.GetType().GetProperty("Id")?.GetValue(result) as Guid?;
            var resultOrigName = result?.GetType().GetProperty("OriginalName")?.GetValue(result)?.ToString();

            if (result == null)
                return ValidationResult.Success;

            if (hasId && (id == null || resultId != id) || hasOrigName && (originalName == null || resultOrigName != originalName))
            {
                _internalMessage = $"{vc.ObjectInstance.GetDisplayName(vc.MemberName)} is already in Use";
                return new ValidationResult(GetErrorMessage(), new[] { vc.MemberName });
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage() => ErrorMessage ?? _internalMessage;
    }
}
