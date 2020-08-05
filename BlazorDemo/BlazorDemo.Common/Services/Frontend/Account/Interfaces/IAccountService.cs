using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BlazorDemo.Common.Services.Frontend.Account.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<LoginUserVM>> LoginAsync(LoginUserVM user);
        Task<ApiResponse<LoginUserVM>> ExternalLoginAuthorizeAsync(LoginUserVM user);
        Task<ApiResponse<RegisterUserVM>> RegisterAsync(RegisterUserVM user);
        Task<ApiResponse<ConfirmUserVM>> ConfirmEmailAsync(string email, string code);
        ApiResponse<FindUserVM> FindUserByEmail(string email);
        Task<ApiResponse<FindUserVM>> FindUserByEmailAsync(string email);
        Task<ApiResponse<IList<AuthenticationScheme>>> GetExternalAuthenticationSchemesAsync();
        Task<ApiResponse<AuthenticateUserVM>> GetAuthenticatedUserAsync();
        Task<ApiResponse<ForgotPasswordUserVM>> ForgotPasswordAsync(ForgotPasswordUserVM user);
        Task<ApiResponse<ResetPasswordUserVM>> ResetPasswordAsync(ResetPasswordUserVM user);
        Task<ApiResponse<ResendEmailConfirmationUserVM>> ResendEmailConfirmationAsync(ResendEmailConfirmationUserVM user);
        Task<IApiResponse> SetFrontendBaseUrlAsync(string frontrndbaseurl);
        ApiResponse<bool> VerifyUserPassword(Guid userId, string password);
        Task<ApiResponse<EditUserVM>> EditAsync(EditUserVM user);
        ApiResponse<FindUserVM> FindUserByName(string name);
        ApiResponse<bool> CheckUserManagerCompliance(string userPropertyName, string userPropertyDisplayName, string userPropertyValue);
        Task<ApiResponse<AuthenticateUserVM>> LogoutAsync();
    }
}
