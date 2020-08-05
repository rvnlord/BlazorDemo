using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Backend.Account.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Api.Controllers
{
    [Route("api/account"), ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        private readonly IApiResponse _defaultInvalidResponse;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
            _defaultInvalidResponse = new ApiResponse(StatusCodeType.Status400BadRequest, "Invalid model", 
                ModelState.Values.SelectMany(v => v.Errors).ToLookup(e => ModelState.Single(s => s.Value.Errors.Contains(e)).Key, e => e.ErrorMessage));
        }

        [HttpPost("login")] // POST: api/account/login
        public async Task<JToken> LoginAsync(LoginUserVM user) => (ModelState.IsValid ? await _accountManager.LoginAsync(user) : _defaultInvalidResponse).ToJToken();

        [HttpGet("externallogin")] // GET: api/account/externallogin
        public async Task<IActionResult> ExternalLoginAsync(string provider, string returnUrl, string rememberMe)
        {
            try
            {
                var (authenticationProperties, schemaName) = await _accountManager.ExternalLoginAsync(new LoginUserVM { ExternalProvider = provider, ReturnUrl = returnUrl.HtmlDecode(), RememberMe = Convert.ToBoolean(rememberMe) });
                return Challenge(authenticationProperties, schemaName);
            }
            catch (ArgumentNullException ex)
            {
                return Redirect(ex.Message);
            }
        }
        
        [HttpGet("externallogincallback")] // GET: api/account/externallogincallback
        public async Task<IActionResult> ExternalLoginCallbackAsync(string returnUrl = null, string remoteError = null) => Redirect(await _accountManager.ExternalLoginCallbackAsync(returnUrl, remoteError));

        [HttpPost("externalloginauthorize")] // POST: api/account/externalloginauthorize
        public async Task<JToken> ExternalLoginAuthorizeAsync(JToken jLoginUserVM) => (ModelState.IsValid ? await _accountManager.ExternalLoginAuthorizeAsync(HttpContext, jLoginUserVM.To<LoginUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("register")] // POST: api/account/register
        public async Task<JToken> RegisterAsync(JToken jRegisterUser) => (ModelState.IsValid ? await _accountManager.RegisterAsync(jRegisterUser.To<RegisterUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("confirmemail")] // POST: api/account/confirmemail
        public async Task<JToken> ConfirmEmailAsync(JToken emailAndCode) => (ModelState.IsValid ? await _accountManager.ConfirmEmailAsync(emailAndCode["email"].ToString(), emailAndCode["code"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("finduserbyemail")] // POST: api/account/finduserbyemail
        public JToken FindUserByEmail(JToken jEmail) => (ModelState.IsValid ? _accountManager.FindUserByEmail(jEmail["email"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("finduserbyemailasync")] // POST: api/account/finduserbyemailasync
        public async Task<JToken> FindUserByEmailAsync(object email) => (ModelState.IsValid ? await _accountManager.FindUserByEmailAsync(email.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("getexternalauthenticationschemes")] // POST: api/account/getexternalauthenticationschemes
        public async Task<JToken> GetExternalAuthenticationSchemesAsync() => (ModelState.IsValid ? await _accountManager.GetExternalAuthenticationSchemesAsync() : _defaultInvalidResponse).ToJToken();

        [HttpPost("authenticateuser")] // POST: api/account/authenticateuser
        public async Task<JToken> GetAuthenticatedUserAsync(JToken jAuthenticateUser) => (ModelState.IsValid ? await _accountManager.GetAuthenticatedUserAsync(HttpContext, User, jAuthenticateUser.To<AuthenticateUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("forgotpassword")] // POST: api/account/forgotpassword
        public async Task<JToken> ForgotPasswordAsync(JToken jForgotPasswordUserVM) => (ModelState.IsValid ? await _accountManager.ForgotPasswordAsync(jForgotPasswordUserVM.To<ForgotPasswordUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("resetpassword")] // POST: api/account/resetpassword
        public async Task<JToken> ResetPasswordAsync(JToken JResetPasswordUserVM) => (ModelState.IsValid ? await _accountManager.ResetPasswordAsync(JResetPasswordUserVM.To<ResetPasswordUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("resendemailconfirmation")] // POST: api/account/resendemailconfirmation
        public async Task<JToken> ResendEmailConfirmationAsync(JToken JResendEmailConfirmationUserVM) => (ModelState.IsValid ? await _accountManager.ResendEmailConfirmationAsync(JResendEmailConfirmationUserVM.To<ResendEmailConfirmationUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("setfrontendbaseurl")] // POST: api/account/setfrontendbaseurl
        public async Task<JToken> SetFrontendBaseUrlAsync(JToken jFrontendBaseUrl) => (ModelState.IsValid ? await _accountManager.SetFrontEndBaseUrl(jFrontendBaseUrl.To<string>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("verifyuserpassword")] // POST: api/account/verifyuserpassword
        public JToken VerifyUserPassword(JToken jUserIdAndPassword) => (ModelState.IsValid ? _accountManager.VerifyUserPassword(jUserIdAndPassword["userId"].To<Guid>(), jUserIdAndPassword["password"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("edit")] // POST: api/account/edit
        public async Task<JToken> EditAsync(JToken JAuthUserAndEditUser) => (ModelState.IsValid ? await _accountManager.EditAsync(JAuthUserAndEditUser["AuthenticatedUser"]?.To<AuthenticateUserVM>(), JAuthUserAndEditUser["UserToEdit"].To<EditUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("finduserbyname")] // POST: api/account/finduserbyname
        public JToken FindUserByName(JToken jName) => (ModelState.IsValid ? _accountManager.FindUserByName(jName["name"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("checkusermanagercompliance")] // POST: api/account/checkusermanagercompliance
        public JToken CheckUserManagerCompliance(JToken jUserPropertyNameUserPropertyDisplayNameAndUserPropertValue) => (ModelState.IsValid ? _accountManager.CheckUserManagerCompliance(jUserPropertyNameUserPropertyDisplayNameAndUserPropertValue["UserPropertyName"]?.ToString(), jUserPropertyNameUserPropertyDisplayNameAndUserPropertValue["UserPropertyDisplayName"]?.ToString(), jUserPropertyNameUserPropertyDisplayNameAndUserPropertValue["UserPropertyValue"]?.ToString()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("logout")] // POST: api/account/logout
        public async Task<JToken> LogoutAsync(JToken JAuthUser) => (ModelState.IsValid ? await _accountManager.LogoutAsync(JAuthUser.To<AuthenticateUserVM>()) : _defaultInvalidResponse).ToJToken();
    }
}
