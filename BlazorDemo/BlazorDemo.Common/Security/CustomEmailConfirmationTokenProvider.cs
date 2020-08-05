using System;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Security;

namespace BlazorDemo.Common.Security
{
    public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : User
    {
        private readonly AppDbContext _db;

        public CustomEmailConfirmationTokenProvider(
            IDataProtectionProvider dataProtectionProvider, 
            IOptions<CustomEmailConfirmationTokenProviderOptions> options,
            ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger,
            AppDbContext db)
            : base(dataProtectionProvider, options, logger)
        {
            _db = db;
        }

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> userManager, TUser user)
        {
            var securityStamp = (await userManager.GetSecurityStampAsync(user)).Take(8).UTF8ToBase64SafeUrl();
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString().UTF8ToBase64SafeUrl();
            var tokenContent = new SecureRandom().GenerateSeed(12).ToBase64SafeUrlString();
            user.EmailActivationToken = $"{timeStamp}|{securityStamp}|{tokenContent}".UTF8ToBase64SafeUrl();
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return  user.EmailActivationToken;
        }

        public override async Task<bool> ValidateAsync(string purpose, string encodedToken, UserManager<TUser> userManager, TUser user)
        {
            var token = encodedToken.Base64SafeUrlToUTF8();
            var tokenParts = token.Split("|");
            var timeStamp = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(tokenParts[0].Base64SafeUrlToUTF8()));
            var securityStamp = tokenParts[1].Base64SafeUrlToUTF8();
            var tokenContent = tokenParts[2];

            var dbEncodedToken = user.EmailActivationToken;
            var dbToken = dbEncodedToken.Base64SafeUrlToUTF8();
            var dbTokenParts = dbToken.Split("|");
            var dbTimeStamp = timeStamp + Options.TokenLifespan;
            var dbSecurityStamp = (await userManager.GetSecurityStampAsync(user)).Take(8);
            var dbTokenContent = dbTokenParts[2];

            var isValid =
                encodedToken.EqualsInvariant(dbEncodedToken) && timeStamp < dbTimeStamp && 
                securityStamp.EqualsInvariant(dbSecurityStamp) && tokenContent.EqualsInvariant(dbTokenContent);

            if (isValid)
            {
                user.EmailActivationToken = null;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }

            return isValid;
        }
    } 

    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    { }
}
