using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace BlazorDemo.Common.Services.Backend.Account.Interfaces
{
    public interface IAccountManager
    {
        Task<ApiResponse<LoginUserVM>> LoginAsync(LoginUserVM user);
        Task<(AuthenticationProperties authenticationProperties, string schemaName)> ExternalLoginAsync(LoginUserVM user);
        Task<string> ExternalLoginCallbackAsync(string returnUrl, string remoteError);
        Task<ApiResponse<LoginUserVM>> ExternalLoginAuthorizeAsync(HttpContext http, LoginUserVM user);
        Task<ApiResponse<RegisterUserVM>> RegisterAsync(RegisterUserVM user);
        Task<ApiResponse<ConfirmUserVM>> ConfirmEmailAsync(string email, string code);
        ApiResponse<FindUserVM> FindUserByEmail(string email);
        Task<ApiResponse<FindUserVM>> FindUserByEmailAsync(string email);
        Task<ApiResponse<IList<AuthenticationScheme>>> GetExternalAuthenticationSchemesAsync();
        Task<ApiResponse<AuthenticateUserVM>> GetAuthenticatedUserAsync(HttpContext http, ClaimsPrincipal principal, AuthenticateUserVM user);
        Task<ApiResponse<ForgotPasswordUserVM>> ForgotPasswordAsync(ForgotPasswordUserVM user);
        Task<ApiResponse<ResetPasswordUserVM>> ResetPasswordAsync(ResetPasswordUserVM user);
        Task<ApiResponse<ResendEmailConfirmationUserVM>> ResendEmailConfirmationAsync(ResendEmailConfirmationUserVM user);
        Task<IApiResponse> SetFrontEndBaseUrl(string frontendBaseUrl);
        ApiResponse<bool> VerifyUserPassword(Guid userId, string password);
        Task<ApiResponse<EditUserVM>> EditAsync(AuthenticateUserVM authUser, EditUserVM user);
        Task<string> GenerateLoginTicketAsync(Guid id, string passwordHash, bool rememberMe);
        ApiResponse<FindUserVM> FindUserByName(string name);
        ApiResponse<bool> CheckUserManagerCompliance(string userPropertyName, string userPropertyDisplayName, string userPropertyValue);
        Task<ApiResponse<AuthenticateUserVM>> LogoutAsync(AuthenticateUserVM authUser);
    }
}
