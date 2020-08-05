using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Common.Services.Frontend.Account
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;

        public AccountService(HttpClient httpClient, IJSRuntime jsRuntime, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
        }

        public static IAccountService Create(HttpClient httpClient)
        {
            return new AccountService(httpClient, null, null);
        }

        public async Task<ApiResponse<LoginUserVM>> LoginAsync(LoginUserVM userToLogin)
        {
            try
            {
                var loginResponse = await _httpClient.PostJTokenAsync<ApiResponse<LoginUserVM>>("api/account/login", userToLogin);
                if (loginResponse.IsError)
                    return loginResponse;

                var loggedUser = loginResponse.Result;
                if (loggedUser.RememberMe)
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", loggedUser.Ticket, new { expires = 365 * 24 * 60 * 60 });
                    await _localStorage.SetItemAsync("Ticket", loggedUser.Ticket);
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", loggedUser.Ticket);
                    await _localStorage.RemoveItemAsync("Ticket");
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception during the Login Attempt", null, null, ex);
            }
        }

        public async Task<ApiResponse<LoginUserVM>> ExternalLoginAuthorizeAsync(LoginUserVM userToExternalLogin)
        {
            try
            {
                var loginResponse = await _httpClient.PostJTokenAsync<ApiResponse<LoginUserVM>>("api/account/externalloginauthorize", userToExternalLogin);
                if (loginResponse.IsError)
                    return loginResponse;

                var loggedUser = loginResponse.Result;
                if (loggedUser.RememberMe)
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", loggedUser.Ticket, new { expires = 365 * 24 * 60 * 60 });
                    await _localStorage.SetItemAsync("Ticket", loginResponse.Result.Ticket);
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", loggedUser.Ticket);
                    await _localStorage.RemoveItemAsync("Ticket");
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception during the ExternalLogin Attempt", null, null, ex);
            }
        }

        public async Task<ApiResponse<RegisterUserVM>> RegisterAsync(RegisterUserVM userToRegister)
        {
            try
            {
                var registerResp = await _httpClient.PostJTokenAsync<ApiResponse<RegisterUserVM>>("api/account/register", userToRegister);
                if (registerResp.IsError)
                    return registerResp;

                if (registerResp.Result.Ticket != null)
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", registerResp.Result.Ticket);
                    await _localStorage.RemoveItemAsync("Ticket");
                }

                return registerResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegisterUserVM>(StatusCodeType.Status500InternalServerError, "Api threw an Exception", null, null, ex);
            }
        }

        public async Task<ApiResponse<ConfirmUserVM>> ConfirmEmailAsync(string email, string code)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<ConfirmUserVM>>("api/account/confirmemail", new { email, code });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status500InternalServerError, "Api threw an Exception", null, null, ex);
            }
        }

        public ApiResponse<FindUserVM> FindUserByEmail(string email) // call inside ValidationAttribute cannot be async
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/account/finduserbyemail", new JObject { ["email"] = email }.JsonSerialize()).JsonDeserialize().To<ApiResponse<FindUserVM>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find User by Email", null, null, ex);
            }
        }

        public async Task<ApiResponse<FindUserVM>> FindUserByEmailAsync(string email)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<FindUserVM>>("api/account/finduserbyemailasync", email);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find a User by Email", null, null, ex);
            }
        }

        public async Task<ApiResponse<IList<AuthenticationScheme>>> GetExternalAuthenticationSchemesAsync()
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<IList<AuthenticationScheme>>>("api/account/getexternalauthenticationschemes"); // or IApiResponse .ToGeneric(jt => jt.To<IList<AuthenticationScheme>>());
            }
            catch (Exception ex)
            {
                return new ApiResponse<IList<AuthenticationScheme>>(StatusCodeType.Status500InternalServerError, "Api threw an Exception", null, null, ex);
            }
        }

        public async Task<ApiResponse<AuthenticateUserVM>> GetAuthenticatedUserAsync()
        {
            try
            {
                var cookieTIcket = await _jsRuntime.InvokeAsync<string>("Cookies.get", "Ticket");
                var localStorageTicket = await _localStorage.GetItemAsStringAsync("Ticket");
                var userToAuthenticate = new AuthenticateUserVM { Ticket = cookieTIcket ?? localStorageTicket, IsAuthenticated = false };
                return await _httpClient.PostJTokenAsync<ApiResponse<AuthenticateUserVM>>("api/account/authenticateuser", userToAuthenticate); 
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while trying to authenticate user", null, AuthenticateUserVM.NotAuthenticated, ex);
            }
        }

        public async Task<ApiResponse<ForgotPasswordUserVM>> ForgotPasswordAsync(ForgotPasswordUserVM forgotPasswordUser)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<ForgotPasswordUserVM>>("api/account/forgotpassword", forgotPasswordUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status500InternalServerError, "Api threw an Exception", null, null, ex);
            }
        }

        public async Task<ApiResponse<ResetPasswordUserVM>> ResetPasswordAsync(ResetPasswordUserVM resetPasswordUserVM)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<ResetPasswordUserVM>>("api/account/resetpassword", resetPasswordUserVM);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status500InternalServerError, "Api threw an Exception while resetting password", null, null, ex);
            }
        }

        public async Task<ApiResponse<ResendEmailConfirmationUserVM>> ResendEmailConfirmationAsync(ResendEmailConfirmationUserVM resendEmailConfirmationUser)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<ResendEmailConfirmationUserVM>>("api/account/resendemailconfirmation", resendEmailConfirmationUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while resending Email Confirmation", null, null, ex);
            }
        }

        public async Task<IApiResponse> SetFrontendBaseUrlAsync(string frontendBaseUrl)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<ResendEmailConfirmationUserVM>>("api/account/setfrontendbaseurl", frontendBaseUrl);
            }
            catch (Exception ex)
            {
                return new ApiResponse(StatusCodeType.Status500InternalServerError, "API threw an exception while setting Frontend Base Url", null, null, ex);
            }
        }

        public ApiResponse<bool> VerifyUserPassword(Guid userId, string password)
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/account/verifyuserpassword", new JObject { ["userId"] = userId, ["password"] = password }.JsonSerialize()).JsonDeserialize().To<ApiResponse<bool>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(StatusCodeType.Status500InternalServerError, "API threw an Exception while verifying if password match an existing one", null, false, ex);
            }
        }

        public async Task<ApiResponse<EditUserVM>> EditAsync(EditUserVM editUser)
        {
            try
            {
                var authUser = (await GetAuthenticatedUserAsync())?.Result;
                var editResp = await _httpClient.PostJTokenAsync<ApiResponse<EditUserVM>>("api/account/edit", new
                {
                    AuthenticatedUser = authUser, 
                    UserToEdit = editUser
                });

                if (editResp.IsError)
                    return editResp;

                if (authUser?.RememberMe == true)
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", editResp.Result.Ticket, new { expires = 365 * 24 * 60 * 60 });
                    await _localStorage.SetItemAsync("Ticket",  editResp.Result.Ticket);
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", editResp.Result.Ticket);
                    await _localStorage.RemoveItemAsync("Ticket");
                }

                return editResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<EditUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while resending editing the current User", null, null, ex);
            }
        }

        public ApiResponse<FindUserVM> FindUserByName(string name)
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/account/finduserbyname", new JObject { ["name"] = name }.JsonSerialize()).JsonDeserialize().To<ApiResponse<FindUserVM>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find User by Name", null, null, ex);
            }
        }

        public ApiResponse<bool> CheckUserManagerCompliance(string userPropertyName, string userPropertyDisplayName, string userPropertyValue)
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/account/checkusermanagercompliance", new JObject
                {
                    ["UserPropertyName"] = userPropertyName, 
                    ["UserPropertyDisplayName"] = userPropertyDisplayName, 
                    ["UserPropertyValue"] = userPropertyValue
                }.JsonSerialize()).JsonDeserialize().To<ApiResponse<bool>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(StatusCodeType.Status500InternalServerError, $"API threw an Exception while checking if property \"{userPropertyName}\" is Compliant with User Manager", null, false, ex);
            }
        }

        public async Task<ApiResponse<AuthenticateUserVM>> LogoutAsync()
        {
            try
            {
                var authUser = (await GetAuthenticatedUserAsync())?.Result;
                var logoutResp = await _httpClient.PostJTokenAsync<ApiResponse<AuthenticateUserVM>>("api/account/logout", authUser);

                if (logoutResp.IsError)
                    return logoutResp;

                await _jsRuntime.InvokeVoidAsync("Cookies.expire", "Ticket");
                await _localStorage.RemoveItemAsync("Ticket");

                return logoutResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while loggin out current user", null, null, ex);
            }
        }
    }
}
