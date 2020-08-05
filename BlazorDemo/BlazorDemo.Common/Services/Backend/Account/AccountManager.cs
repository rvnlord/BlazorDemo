using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Converters.Collections;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Backend.Account.Interfaces;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace BlazorDemo.Common.Services.Backend.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSenderManager _emailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountManager(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSenderManager emailSender,
            AppDbContext db,
            IMapper autoMapper,
            IHttpContextAccessor http,
            IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _db = db;
            _mapper = autoMapper;
            _http = http;
            _passwordHasher = passwordHasher;
        }

        public AccountManager(AppDbContext db)
        {
            _db = db; // for fallback init if di is not available (in ValidationAttributes for instance)
        }

        public async Task<ApiResponse<LoginUserVM>> LoginAsync(LoginUserVM userToLogin)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userToLogin.Email);
                if (user == null)
                    return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, "Email not found, please Register first", new[] { new KeyValuePair<string, string>("Email", "There is no User with this Email") }.ToLookup());

                userToLogin.UserName = user.UserName;
                if (!user.EmailConfirmed && await _userManager.CheckPasswordAsync(user, userToLogin.Password) && _userManager.Options.SignIn.RequireConfirmedEmail)
                    return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, "Confirm your account by clicking the link in your email first", new[] { new KeyValuePair<string, string>("Email", "Email is not confirmed yet") }.ToLookup());

                var loginResult = await _signInManager.PasswordSignInAsync(userToLogin.UserName, userToLogin.Password, userToLogin.RememberMe, true);
                if (!loginResult.Succeeded)
                {
                    var message = string.Empty;
                    var property = nameof(userToLogin.Email);
                    if (loginResult == SignInResult.Failed)
                    {
                        var failedLoginAttempts = await _userManager.GetAccessFailedCountAsync(user);
                        var maxLoginAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                        property = nameof(userToLogin.Password);
                        if (user.PasswordHash != null)
                            message = $"Incorrect Password ({maxLoginAttempts - failedLoginAttempts} attempts left)";
                        else
                            message = $"You don't have any password set for your Account, Please log in with your External Provider and set one or use Reset Password Form in case you don't have access to your external Account ({maxLoginAttempts - failedLoginAttempts} attempts left)";
                    }
                    else if (loginResult == SignInResult.NotAllowed)
                        message = "You don't have permission to Sign-In";
                    else if (loginResult == SignInResult.LockedOut)
                    {
                        var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user) ?? new DateTimeOffset(); // never null because lockout flag is true at this point
                        var lockoutTimeLeft = lockoutEndDate - DateTime.UtcNow;
                        message = $"Account Locked, too many failed attempts (try again in: {lockoutTimeLeft.Minutes}m {lockoutTimeLeft.Seconds}s)";
                    }
                    else if (loginResult == SignInResult.TwoFactorRequired)
                        message = "2FA Code hasn't been provided";
                    return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, $"Login Failed - {message}", new[] { new KeyValuePair<string, string>(property, message) }.ToLookup());
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                userToLogin.Ticket = await GenerateLoginTicketAsync(user.Id, user.PasswordHash, userToLogin.RememberMe);
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status200OK, $"You are now logged in as \"{userToLogin.UserName}\"", null, _mapper.Map(user, userToLogin));
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status500InternalServerError, "Login Failed", null, null, ex);
            }
        }

        public async Task<string> GenerateLoginTicketAsync(Guid id, string passwordHash, bool rememberMe)
        {
            var key = (await _db.CryptographyKeys.SingleOrDefaultAsync(k => k.Name.ToLower() == "LoginTicket"))?.Value;
            if (key == null)
            {
                key = CryptoUtils.CreateCamelliaKey().ToBase64SafeUrlString();
                await _db.CryptographyKeys.AddAsync(new CryptographyKey { Name = "LoginTicket", Value = key });
                await _db.SaveChangesAsync();
            }

            return $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}|{id}|{passwordHash}|{Convert.ToInt32(rememberMe)}".UTF8ToByteArray()
                .EncryptCamellia(key.Base64SafeUrlToByteArray()).ToBase64SafeUrlString();
        }

        public async Task<(AuthenticationProperties authenticationProperties, string schemaName)> ExternalLoginAsync(LoginUserVM userToExternalLogin)
        {
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            var scheme = schemes.SingleOrDefault(s => s.Name.EqualsInvariantIgnoreCase(userToExternalLogin.ExternalProvider));
            var frontEndBaseurl = ConfigUtils.FrontendBaseUrl;
            if (scheme == null)
                throw new ArgumentNullException(null, $"{frontEndBaseurl}Account/Login?remoteStatus=Error&remoteMessage={"Provider not Found".UTF8ToBase64SafeUrl()}");

            var reqScheme = _http.HttpContext.Request.Scheme; // we need API url here which is different than the Web App one
            var host = _http.HttpContext.Request.Host;
            var pathbase = _http.HttpContext.Request.PathBase;
            var redirectUrl = $"{reqScheme}://{host}{pathbase}/api/account/externallogincallback?returnUrl={userToExternalLogin.ReturnUrl.HtmlEncode()}&user={userToExternalLogin.JsonSerialize().UTF8ToBase64SafeUrl()}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(userToExternalLogin.ExternalProvider, redirectUrl);

            return (properties, scheme.Name);
        }

        public async Task<string> ExternalLoginCallbackAsync(string returnUrl, string remoteError)
        {
            var userToExternalLogin = _http.HttpContext.Request.Query["user"].ToString().Base64SafeUrlToUTF8().JsonDeserialize().To<LoginUserVM>();
            var loggedInUrl = returnUrl.BeforeFirstOrWhole("?");
            var frontEndBaseurl = ConfigUtils.FrontendBaseUrl;
            var url = $"{frontEndBaseurl}Account/Login";
            var qs = loggedInUrl.QueryStringToDictionary();
            qs["remoteStatus"] = "Error";

            try
            {
                if (remoteError != null)
                {
                    qs["remoteMessage"] = remoteError.UTF8ToBase64SafeUrl();
                    return $"{url}?{qs.ToQueryString()}";
                }

                var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    qs["remoteMessage"] = "Error loading external login information".UTF8ToBase64SafeUrl();
                    return $"{url}?{qs.ToQueryString()}";
                }

                userToExternalLogin.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                userToExternalLogin.UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? userToExternalLogin.Email.BeforeFirst("@");
                userToExternalLogin.ExternalProvider = externalLoginInfo.LoginProvider;
                userToExternalLogin.ExternalProviderKey = externalLoginInfo.ProviderKey;
                qs["user"] = userToExternalLogin.JsonSerialize().UTF8ToBase64SafeUrl();
                qs.Remove("remoteStatus");
                return $"{url}?{qs.ToQueryString()}";
            }
            catch (Exception ex)
            {
                qs["remoteMessage"] = $"Retrieving Provider Key Failed - {ex.Message}".UTF8ToBase64SafeUrl();
                return $"{url}?{qs.ToQueryString()}";
            }
        }

        public async Task<ApiResponse<LoginUserVM>> ExternalLoginAuthorizeAsync(HttpContext http, LoginUserVM userToExternalLogin)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userToExternalLogin.Email);
                if (user == null)
                {
                    var userToRegister = new RegisterUserVM { UserName = userToExternalLogin.UserName, Email = userToExternalLogin.Email };
                    var registerUserResponse = await RegisterAsync(userToRegister);
                    if (registerUserResponse.IsError)
                        return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, registerUserResponse.Message, null);
                    user = await _userManager.FindByEmailAsync(userToExternalLogin.Email);
                }

                if (!user.EmailConfirmed && _userManager.Options.SignIn.RequireConfirmedEmail)
                    return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, "Please Confirm your email first", null);

                var externalLoginResult = await _signInManager.ExternalLoginSignInAsync(userToExternalLogin.ExternalProvider, userToExternalLogin.ExternalProviderKey, userToExternalLogin.RememberMe, true);
                if (!externalLoginResult.Succeeded)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(userToExternalLogin.ExternalProvider, userToExternalLogin.ExternalProviderKey, userToExternalLogin.ExternalProvider));
                    if (!addLoginResult.Succeeded)
                    {
                        var message = "There was an Unknown Error during External User Login";
                        if (externalLoginResult == SignInResult.NotAllowed)
                            message = "You don't have permission to Sign-In with an External Provider";
                        if (externalLoginResult == SignInResult.LockedOut)
                        {
                            var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user) ?? new DateTimeOffset(); // never null because lockout flag is true at this point
                            var lockoutTimeLeft = lockoutEndDate - DateTime.UtcNow;
                            message = $"Account Locked, too many failed attempts (try again in: {lockoutTimeLeft.Minutes}m {lockoutTimeLeft.Seconds}s)";
                        }

                        return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, message, null);
                    }

                    var secondAttemptExternalLoginResult = await _signInManager.ExternalLoginSignInAsync(userToExternalLogin.ExternalProvider, userToExternalLogin.ExternalProviderKey, userToExternalLogin.RememberMe, true);
                    if (!secondAttemptExternalLoginResult.Succeeded)
                        return new ApiResponse<LoginUserVM>(StatusCodeType.Status401Unauthorized, "User didn't have an External Login Account so it was added, but Login Attempt has Failed", null);
                }

                await _signInManager.SignInAsync(user, userToExternalLogin.RememberMe);
                await _userManager.ResetAccessFailedCountAsync(user);
                userToExternalLogin.Ticket = await GenerateLoginTicketAsync(user.Id, user.PasswordHash, userToExternalLogin.RememberMe);
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status200OK, $"You have been successfully logged in with an External Provider as: \"{_mapper.Map(user, userToExternalLogin).UserName}\"", null, userToExternalLogin);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginUserVM>(StatusCodeType.Status500InternalServerError, "External Login Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<RegisterUserVM>> RegisterAsync(RegisterUserVM userToRegister)
        {
            try
            {
                var user = new User { UserName = userToRegister.UserName, Email = userToRegister.Email };
                var result = userToRegister.Password != null ? await _userManager.CreateAsync(user, userToRegister.Password) : await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.ToLookup(user.GetPropertyNames());
                    return new ApiResponse<RegisterUserVM>(StatusCodeType.Status401Unauthorized, "Invalid Model", errors, null);
                }

                await _userManager.AddClaimAsync(user, new Claim("Email", user.Email));
                await _userManager.AddToRoleAsync(user, "User");

                if (_userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    var code = (await _userManager.GenerateEmailConfirmationTokenAsync(user)).UTF8ToBase64SafeUrl();
                    var deployingEmailResponse = await _emailSender.SendConfirmationEmailAsync(userToRegister.Email, code, userToRegister.ReturnUrl);
                    if (deployingEmailResponse.IsError)
                    {
                        userToRegister.ReturnUrl = $"/Account/ResendEmailConfirmation/?email={userToRegister.Email}&returnUrl={userToRegister.ReturnUrl.HtmlEncode()}";
                        return new ApiResponse<RegisterUserVM>(StatusCodeType.Status500InternalServerError, "Registration had been Successful, but the email wasn't sent. Try again later.", null, userToRegister, deployingEmailResponse.ResponseException);
                    }

                    userToRegister.ReturnUrl = $"/Account/ConfirmEmail?email={userToRegister.Email}&returnUrl={userToRegister.ReturnUrl.HtmlEncode()}";
                    return new ApiResponse<RegisterUserVM>(StatusCodeType.Status201Created, $"Registration for User \"{userToRegister.UserName}\" has been successful, activation email has been deployed to: \"{userToRegister.Email}\"", null, _mapper.Map(userToRegister, new RegisterUserVM()));
                }

                userToRegister.ReturnUrl = $"/Account/Login?returnUrl={userToRegister.ReturnUrl.HtmlEncode()}";
                userToRegister.Ticket = await GenerateLoginTicketAsync(user.Id, user.PasswordHash, false);
                await _signInManager.SignInAsync(user, false);
                return new ApiResponse<RegisterUserVM>(StatusCodeType.Status201Created, $"Registration for User \"{userToRegister.UserName}\" has been successful, you are now logged in", null, userToRegister);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegisterUserVM>(StatusCodeType.Status500InternalServerError, "Registration Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<ConfirmUserVM>> ConfirmEmailAsync(string email, string code)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status401Unauthorized, "There is no User with this Email to Confirm", new[] { new KeyValuePair<string, string>("Email", "No such email") }.ToLookup());

                var isAlreadyConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (isAlreadyConfirmed)
                    return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status401Unauthorized, "Email is confirmed", new[] { new KeyValuePair<string, string>("Email", "Email has already been confirmed") }.ToLookup());

                if (!code.Base64SafeUrlToUTF8().IsBase64()) // we decode twice (once inside) so we can throw from here if code decoded once is not base64
                    return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status400BadRequest, "Email confirmation failed", new[] { new KeyValuePair<string, string>("ConfirmationCode", "Confirmation Code is Invalid") }.ToLookup());

                var confirmationResult = await _userManager.ConfirmEmailAsync(user, code.Base64SafeUrlToUTF8());
                if (!confirmationResult.Succeeded)
                {
                    var errors = confirmationResult.Errors.ToSinglePropertyLookup("ConfirmationCode");
                    return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status401Unauthorized, "Email confirmation failed", errors);
                }
                return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status200OK, "Email has been Confirmed Successfully", null, _mapper.Map(user, new ConfirmUserVM()));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmUserVM>(StatusCodeType.Status500InternalServerError, "Confirming Email Failed", null, null, ex);
            }
        }

        public ApiResponse<FindUserVM> FindUserByEmail(string email)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                    return new ApiResponse<FindUserVM>(StatusCodeType.Status404NotFound, "There is no User with the given Email", null);

                var foundUser = _mapper.Map(user, new FindUserVM()); // claims and roles are more difficult to retrieve in sync context, however we don't need them in an attribute where this method is called
                return new ApiResponse<FindUserVM>(StatusCodeType.Status200OK, "User Found", null, foundUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "Finding User by Email Failed", null, null, ex);
            }
        }

        public ApiResponse<FindUserVM> FindUserByName(string name)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.UserName.ToLower() == name.ToLower());
                if (user == null)
                    return new ApiResponse<FindUserVM>(StatusCodeType.Status404NotFound, "There is no User with the given Name", null);

                var foundUser = _mapper.Map(user, new FindUserVM()); // claims and roles are more difficult to retrieve in sync context, however we don't need them in an attribute where this method is called
                return new ApiResponse<FindUserVM>(StatusCodeType.Status200OK, "User Found", null, foundUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "Finding User by Name Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<FindUserVM>> FindUserByEmailAsync(string email)
        {
            try
            {
                var user = await _db.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                    return new ApiResponse<FindUserVM>(StatusCodeType.Status404NotFound, "There is no User with the given Email", null);

                var foundUser = _mapper.Map(user, new FindUserVM());
                foundUser.Roles = (await _userManager.GetRolesAsync(user)).Select(r => new FindRoleVM { Name = r }).ToList();
                foundUser.Claims = (await _userManager.GetClaimsAsync(user)).Select(c => new FindClaimVM { Name = c.Type }).Where(c => !c.Name.EqualsIgnoreCase("Email")).ToList();

                return new ApiResponse<FindUserVM>(StatusCodeType.Status200OK, "Finding User by Email has been Successful", null, foundUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "Finding User by Email Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<IList<AuthenticationScheme>>> GetExternalAuthenticationSchemesAsync()
        {
            try
            {
                var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return new ApiResponse<IList<AuthenticationScheme>>(StatusCodeType.Status200OK, "External Authentication Schemes Returned", null, externalLogins);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IList<AuthenticationScheme>>(StatusCodeType.Status500InternalServerError, "Failed Getting Authentication Schemes", null, null, ex);
            }
        }

        public async Task<ApiResponse<AuthenticateUserVM>> GetAuthenticatedUserAsync(HttpContext http, ClaimsPrincipal principal, AuthenticateUserVM userToAuthenticate)
        {
            try
            {
                userToAuthenticate.IsAuthenticated = false;
                var contextPrincipal = http != null ? (await http.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Principal : null;
                var principals = new[] { principal, contextPrincipal };
                var claimsPrincipal = principals.FirstOrDefault(p => p?.Identity?.Name != null && p.Identity.IsAuthenticated);
                
                if (userToAuthenticate.Ticket.IsNullOrWhiteSpace())
                {
                    await _signInManager.SignOutAsync();
                    return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status200OK, "User is not Authenticated", null, userToAuthenticate);
                }

                var key = (await _db.CryptographyKeys.SingleOrDefaultAsync(k => k.Name == "LoginTicket"))?.Value?.Base64SafeUrlToByteArray();
                var decryptedTicket = userToAuthenticate.Ticket.Base64SafeUrlToByteArray().DecryptCamellia(key).ToBase64SafeUrlString().Base64SafeUrlToUTF8().Split("|");
                var timeStamp = Convert.ToInt64(decryptedTicket[0]).UnixTimeStampToDateTime();
                var id = decryptedTicket[1];
                var passwordHash = decryptedTicket[2].IsNullOrWhiteSpace() ? null : decryptedTicket[2]; // we need to nullify the empty string that comes from a decrypted ticket because otherwise we would get passwordHash ("") == user.PasswordHash (null) = false
                var rememberMe = Convert.ToBoolean(Convert.ToInt32(decryptedTicket[3]));

                User user = null;
                if (claimsPrincipal?.Identity?.Name != null)
                    user = await _userManager.FindByNameAsync(claimsPrincipal.Identity.Name);
                if (user == null)
                {
                    user = await _userManager.FindByIdAsync(id);
                    if (user == null || !id.EqualsInvariant(user.Id.ToString()) || !passwordHash.EqualsInvariant(user.PasswordHash) || DateTimeOffset.UtcNow - timeStamp >= TimeSpan.FromDays(365))
                    {
                        await _signInManager.SignOutAsync();
                        return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status200OK, "User is not Authenticated", null, userToAuthenticate);
                    }

                    await _signInManager.SignInAsync(user, true);
                }

                _mapper.Map(user, userToAuthenticate);
                userToAuthenticate.RememberMe = rememberMe;
                userToAuthenticate.HasPassword = user.PasswordHash != null;
                userToAuthenticate.IsAuthenticated = true;
                userToAuthenticate.Roles = (await _userManager.GetRolesAsync(user)).Select(r => new FindRoleVM { Name = r }).ToList();
                userToAuthenticate.Claims = (await _userManager.GetClaimsAsync(user)).Select(c => new FindClaimVM { Name = c.Type }).Where(c => !c.Name.EqualsIgnoreCase("Email")).ToList();
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status200OK, "Getting Authenticated User was Successful", null, userToAuthenticate);
            }
            catch
            {
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status500InternalServerError, "Getting Authenticated User Failed", null);
            }
        }

        public async Task<ApiResponse<ForgotPasswordUserVM>> ForgotPasswordAsync(ForgotPasswordUserVM forgotPasswordUser)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordUser.Email);
                if (user == null)
                    return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status401Unauthorized, "Email not found, please Register first", new[] { new KeyValuePair<string, string>("Email", "There is no User with this Email") }.ToLookup());
                if (!await _userManager.IsEmailConfirmedAsync(user))
                    return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status401Unauthorized, "Please Confirm your account first", new[] { new KeyValuePair<string, string>("Email", "Your account hasn't been confirmed yet") }.ToLookup());

                var code = (await _userManager.GeneratePasswordResetTokenAsync(user)).UTF8ToBase64SafeUrl();
                var sendResetEmailResponse = await _emailSender.SendPasswordResetEmailAsync(forgotPasswordUser.Email, code, forgotPasswordUser.ReturnUrl);
                if (sendResetEmailResponse.IsError)
                    return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status500InternalServerError, "Can't send Password Reset email. Try again later.", null, null, sendResetEmailResponse.ResponseException);

                return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status201Created, "Reset Password Email has been sent succesfully", null, forgotPasswordUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ForgotPasswordUserVM>(StatusCodeType.Status500InternalServerError, "Sending Reset Password Email Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<ResetPasswordUserVM>> ResetPasswordAsync(ResetPasswordUserVM userToResetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userToResetPassword.Email);
                if (user == null)
                    return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status401Unauthorized, "There is no User with this Email to Confirm", new[] { new KeyValuePair<string, string>("Email", "No such email") }.ToLookup());

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, userToResetPassword.ResetPasswordCode.Base64SafeUrlToUTF8(), userToResetPassword.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    var isInvalidToken = resetPasswordResult.Errors.FirstOrDefault(e => e.Code.EqualsInvariant("InvalidToken"));
                    if (isInvalidToken != null)
                        return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status401Unauthorized, isInvalidToken.Description, new[] { new KeyValuePair<string, string>(nameof(ResetPasswordUserVM.ResetPasswordCode), isInvalidToken.Description) }.ToLookup());
                    var errors = resetPasswordResult.Errors.ToLookup(userToResetPassword.GetPropertyNames());
                    return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status401Unauthorized, "Password Reset Failed", errors);
                }

                if (await _userManager.IsLockedOutAsync(user))
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

                return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status200OK, $"Password for User: \"{userToResetPassword.UserName}\" has been changed", null, _mapper.Map(user, userToResetPassword));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResetPasswordUserVM>(StatusCodeType.Status500InternalServerError, "Resetting Password Failed", null, null, ex);
            }
        }

        public async Task<ApiResponse<ResendEmailConfirmationUserVM>> ResendEmailConfirmationAsync(ResendEmailConfirmationUserVM userToResendEmailConfirmation)
        {
            try
            {
                var successMessage = $"Confirmation email has been sent to: \"{userToResendEmailConfirmation.Email}\" if there is an account associated with it";
                userToResendEmailConfirmation.ReturnUrl += $"?email={userToResendEmailConfirmation.Email}";
                var emailReturnUrl = $"{ConfigUtils.FrontendBaseUrl}Account/Login";
                var user = await _userManager.FindByEmailAsync(userToResendEmailConfirmation.Email);
                if (user == null)
                    return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status200OK, successMessage, null, userToResendEmailConfirmation);

                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    userToResendEmailConfirmation.ReturnUrl = emailReturnUrl;
                    return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status200OK, $"Email \"{userToResendEmailConfirmation.Email}\" has already been confirmed", null, userToResendEmailConfirmation);
                }

                var code = (await _userManager.GenerateEmailConfirmationTokenAsync(user)).UTF8ToBase64SafeUrl();
                var resendingConfirmationEmailResponse = await _emailSender.SendConfirmationEmailAsync(userToResendEmailConfirmation.Email, code, emailReturnUrl);
                if (resendingConfirmationEmailResponse.IsError)
                    return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status500InternalServerError, "Can't resend Confirmation Email. Please try again later.", null, null, resendingConfirmationEmailResponse.ResponseException);

                return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status200OK, successMessage, null, _mapper.Map(user, userToResendEmailConfirmation));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResendEmailConfirmationUserVM>(StatusCodeType.Status500InternalServerError, "Resending Email Confirmation Failed", null, null, ex);
            }
        }

        public async Task<IApiResponse> SetFrontEndBaseUrl(string frontendBaseUrl)
        {
            try
            {
                ConfigUtils.FrontendBaseUrl = frontendBaseUrl;
                return await Task.FromResult(new ApiResponse(StatusCodeType.Status200OK, "Front End Base url Value Set Successfully", null));
            }
            catch (Exception ex)
            {
                return new ApiResponse(StatusCodeType.Status500InternalServerError, "Failed Setting Front End Base url Value", null, null, ex);
            }
        }

        public ApiResponse<bool> VerifyUserPassword(Guid userId, string password)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.Id == userId); // can't use _userManager directly because we cannot work with validation atributes in async context
                if (user == null)
                    return new ApiResponse<bool>(StatusCodeType.Status404NotFound, "There is no User with this id, this should not have happened", null);

                var isPasswordCorrect = user.PasswordHash == null && password == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success;
                return new ApiResponse<bool>(StatusCodeType.Status200OK, "Password Correct", null, isPasswordCorrect);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(StatusCodeType.Status500InternalServerError, "Failed Verifying Password", null, false, ex);
            }
        }

        public async Task<ApiResponse<EditUserVM>> EditAsync(AuthenticateUserVM authUser, EditUserVM userToEdit)
        {
            try
            {
                if (authUser == null || !authUser.IsAuthenticated)
                    return new ApiResponse<EditUserVM>(StatusCodeType.Status401Unauthorized, "You are not Authorized to Edit User Data", null);
                if (userToEdit.Id == default)
                    return new ApiResponse<EditUserVM>(StatusCodeType.Status404NotFound, "Id wasn't supplied", new[] { new KeyValuePair<string, string>("Id", "Id is empty") }.ToLookup());

                var user = await _userManager.FindByIdAsync(userToEdit.Id.ToString());
                if (userToEdit.Id == default)
                    return new ApiResponse<EditUserVM>(StatusCodeType.Status404NotFound, "There is no User with this Id", new[] { new KeyValuePair<string, string>("Id", "There is no User with the supplied Id") }.ToLookup());

                var isConfirmationRequired = !userToEdit.Email.EqualsInvariant(user.Email) && _userManager.Options.SignIn.RequireConfirmedEmail;
                user.Email = userToEdit.Email.IsNullOrWhiteSpace() ? user.Email : userToEdit.Email;
                user.UserName = userToEdit.UserName.IsNullOrWhiteSpace() ? user.UserName : userToEdit.UserName;
                await _userManager.UpdateAsync(user);

                if (!userToEdit.NewPassword.IsNullOrWhiteSpace() && !userToEdit.NewPassword.EqualsInvariant(userToEdit.OldPassword))
                {
                    user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userToEdit.Id);
                    if (user.PasswordHash != null && _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userToEdit.OldPassword) != PasswordVerificationResult.Success) // user should be allowed to change password if he didn't set one at all (was logging in exclusively with an external provider) or if he provided correct Old Password to his Account
                        return new ApiResponse<EditUserVM>(StatusCodeType.Status401Unauthorized, "Old Password is not Correct", new[] { new KeyValuePair<string, string>("OldPassword", "Incorrect Password") }.ToLookup());

                    var errors = new List<IdentityError>();
                    foreach (var v in _userManager.PasswordValidators)
                        errors.AddRange((await v.ValidateAsync(_userManager, user, userToEdit.NewPassword)).Errors);
                    if (errors.Any())
                        return new ApiResponse<EditUserVM>(StatusCodeType.Status401Unauthorized, "New Password is Invalid", errors.ToLookup(userToEdit.GetPropertyNames().Append("Password")).RenameKey("Password", nameof(userToEdit.NewPassword)));

                    user.PasswordHash = _passwordHasher.HashPassword(user, userToEdit.NewPassword); // use db directly to override identity validation because we want to be able to provide password  for a null hash if user didn't set password before
                    userToEdit.Ticket = await GenerateLoginTicketAsync(userToEdit.Id, user.PasswordHash, authUser.RememberMe);
                    await _db.SaveChangesAsync();
                }

                if (isConfirmationRequired)
                {
                    user.EmailConfirmed = false;
                    await _db.SaveChangesAsync();
                    var resendConfirmationResult = await ResendEmailConfirmationAsync(_mapper.Map(userToEdit, new ResendEmailConfirmationUserVM()));
                    if (resendConfirmationResult.IsError)
                        return new ApiResponse<EditUserVM>(StatusCodeType.Status400BadRequest, "User Details have been Updated buy system can't resend Confirmation Email. Please try again later.", null);

                    userToEdit.ReturnUrl = $"{ConfigUtils.FrontendBaseUrl}/Account/ConfirmEmail?email={user.Email}";
                    await _signInManager.SignOutAsync();
                    return new ApiResponse<EditUserVM>(StatusCodeType.Status202Accepted, $"Successfully updated User \"{userToEdit.UserName}\", since you have updated your email address the confirmation code has been sent to: \"{userToEdit.Email}\"", null, userToEdit);
                }

                await _signInManager.SignInAsync(user, true);
                return new ApiResponse<EditUserVM>(StatusCodeType.Status202Accepted, $"Successfully updated User \"{userToEdit.UserName}\"", null, userToEdit);
            }
            catch (Exception ex)
            {
                return new ApiResponse<EditUserVM>(StatusCodeType.Status500InternalServerError, "Editing User Failed", null, null, ex);
            }
        }

        public ApiResponse<bool> CheckUserManagerCompliance(string userPropertyName, string userPropertyDisplayName, string userPropertyValue)
        {
            try // this method is called in an attribute, so we can't use validators directly because they only support async context
            {
                if (userPropertyValue == null)
                    return new ApiResponse<bool>(StatusCodeType.Status200OK, $"{userPropertyDisplayName} is Empty, falling through to other attributes", null, true);

                if (userPropertyName.ContainsIgnoreCase("UserName"))
                {
                    if (!userPropertyValue.All(c => c.In(_userManager.Options.User.AllowedUserNameCharacters))) // we are skipping unique email check here because we are already checking email in other attrbiute and because email is not part of username property despite the fact that usermanager have email option under User category for some reason
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} contains disallowed characters, allowed characters are: [{_userManager.Options.User.AllowedUserNameCharacters.Select(c => $"\'{c}\'").JoinAsString(", ")}]", null, false);
                }

                if (userPropertyName.ContainsIgnoreCase("Password"))
                {
                    if (_userManager.Options.Password.RequireDigit && !userPropertyValue.Any(char.IsDigit))
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} need to contain at least one digit", null, false);
                    if (_userManager.Options.Password.RequireLowercase && !userPropertyValue.Any(char.IsLower))
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} need to contain at least one lower case character", null, false);
                    if (_userManager.Options.Password.RequireUppercase && !userPropertyValue.Any(char.IsUpper))
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} need to contain at least one upper case character", null, false);
                    if (_userManager.Options.Password.RequireNonAlphanumeric && userPropertyValue.All(char.IsLetterOrDigit))
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} need to contain at least one non-alphanumeric character", null, false);
                    if (_userManager.Options.Password.RequiredLength > userPropertyValue.Length)
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} has to be at least {_userManager.Options.Password.RequiredLength} characters long", null, false);
                    if (_userManager.Options.Password.RequiredUniqueChars > userPropertyValue.Distinct().Count())
                        return new ApiResponse<bool>(StatusCodeType.Status400BadRequest, $"{userPropertyDisplayName} has to contain at least {_userManager.Options.Password.RequiredUniqueChars} unique characters", null, false);
                }

                return new ApiResponse<bool>(StatusCodeType.Status200OK, $"{userPropertyDisplayName} is User Manager Compliant", null, true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(StatusCodeType.Status500InternalServerError, "Failed Verifying Password", null, false, ex);
            }
        }

        public async Task<ApiResponse<AuthenticateUserVM>> LogoutAsync(AuthenticateUserVM authUser)
        {
            try
            {
                if (authUser == null || !authUser.IsAuthenticated)
                    return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status401Unauthorized, "You are not Authorized so you can't log out", null);

                await _signInManager.SignOutAsync();
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status200OK, $"You (\"{authUser.UserName}\") have been successfully logged out ", null, authUser);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthenticateUserVM>(StatusCodeType.Status500InternalServerError, "Loggin User out has Failed", null, null, ex);
            }
        }
    }
}
