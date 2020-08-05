using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Converters.Collections;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorDemo.Common.Services.Frontend.Account
{
    public class UserAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccountService _accountService;

        public UserAuthenticationStateProvider(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authenticationResponse = await _accountService.GetAuthenticatedUserAsync();
            if (authenticationResponse.IsError)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var authenticatingUser = authenticationResponse.Result;
            if (!authenticatingUser.IsAuthenticated)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = new[] { new Claim(ClaimTypes.Name, authenticatingUser.UserName) }.Concat(authenticatingUser.Claims.ToNameValueList().Select(c => new Claim(c.Item1, c.Item2)));
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void StateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
