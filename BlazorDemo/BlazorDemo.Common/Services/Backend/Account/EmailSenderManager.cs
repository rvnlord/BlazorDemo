using System;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Backend.Account.Interfaces;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BlazorDemo.Common.Services.Backend.Account
{
    public class EmailSenderManager : IEmailSenderManager
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public EmailSenderManager(UserManager<User> userManager, IConfiguration config, AppDbContext db)
        {
            _userManager = userManager;
            _config = config;
            _db = db;
        }

        public async Task<IApiResponse> SendConfirmationEmailAsync(string email, string code, string returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var verifyUrl = HtmlEncoder.Default.Encode($"{ConfigUtils.FrontendBaseUrl}Account/ConfirmEmail?email={email}&code={code}&returnUrl={returnUrl.HtmlEncode()}"); // can't be `{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host}{_http.HttpContext.Request.PathBase}` because the backend address is different, on local machine it is just a port, on a server it might be a completely different domain

            var sbEmailBody = new StringBuilder();
            sbEmailBody.Append("Hello " + user.UserName + ",<br/><br/>");
            sbEmailBody.Append("You have asked for activation in our Web App. You can activate your account either by providing the activation code directly or by clicking the following link.");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Activation Link:");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append($"<a href='{verifyUrl}'>{verifyUrl}</a></b>");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Activation Code:");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append("<b>" + code + "</b>");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Cheers");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append("My Blazor Demo");

            return await SendEmailAsync(user, "My Blazor Demo - Confirmation Email", sbEmailBody.ToString());
        }

        public async Task<IApiResponse> SendPasswordResetEmailAsync(string email, string code, string returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var resetPasswordurl = HtmlEncoder.Default.Encode($"{ConfigUtils.FrontendBaseUrl}Account/ResetPassword?email={email}&code={code}&returnUrl={returnUrl.HtmlEncode()}");

            var sbEmailBody = new StringBuilder();
            sbEmailBody.Append("Hello " + user.UserName + ",<br/><br/>");
            sbEmailBody.Append("You have asked for password reset in our Web App. You can do so with the code below or by following the Password Reset Link.");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Reset Password Link:");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append($"<a href='{resetPasswordurl}'>{resetPasswordurl}</a></b>");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Reset Password Code:");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append("<b>" + code + "</b>");
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("Cheers");
            sbEmailBody.Append("<br/>");
            sbEmailBody.Append("My Blazor Demo");

            return await SendEmailAsync(user, "My Blazor Demo - Reset Password", sbEmailBody.ToString());
        }

        private async Task<IApiResponse> SendEmailAsync(User user, string subject, string content)
        {
            try
            {
                var emailConfig = _config.GetSection("Email");
                var from = emailConfig.GetSection("From").Value;
                var host = emailConfig.GetSection("Host").Value;
                var port = Convert.ToInt32(emailConfig.GetSection("Port").Value);
                var userName = emailConfig.GetSection("UserName").Value;
                var password = emailConfig.GetSection("Password").Value;
                var ssl = Convert.ToBoolean(emailConfig.GetSection("SSL").Value);

                var passwordKey = (await _db.CryptographyKeys.AsNoTracking().FirstOrDefaultAsync(k => k.Name == "EmailPassword"))?.Value;
                var decryptedPassword = passwordKey == null 
                    ? password 
                    : password.Base64ToByteArray().DecryptCamellia(passwordKey.Base64ToByteArray()).ToUTF8String();

                var key = CryptoUtils.CreateCamelliaKey();
                var encryptedPassword = decryptedPassword.UTF8ToByteArray().EncryptCamellia(key).ToBase64String();
                _db.CryptographyKeys.AddOrUpdate(new CryptographyKey { Name = "EmailPassword", Value = key.ToBase64String() }, e => e.Name);
                await _db.SaveChangesAsync();
                await ConfigUtils.SetAppSettingValueAsync("Email:Password", encryptedPassword);

                var mailMessage = new MailMessage(from, user.Email)
                {
                    IsBodyHtml = true,
                    Body = content,
                    Subject = subject
                };

                var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential { UserName = userName, Password = decryptedPassword },
                    EnableSsl = ssl
                };

                await smtp.SendMailAsync(mailMessage); // if this throws 5.7.0, go here: https://g.co/allowaccess

                return new ApiResponse(StatusCodeType.Status200OK, "Email Sent Successfully", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse(StatusCodeType.Status500InternalServerError, "Sending Email Failed", null, null, ex);
            }
        }
    }
}
