using System;
using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BlazorDemo.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        public string PreviousUrl { get; set; }
        public string CurrentUrl { get; set; }
        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public PromptType PromptStatus { get; set; }
        public string PromptMessage { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider) AuthStateProvider;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ConfigUtils.FrontendBaseUrl = NavigationManager.BaseUri;
            await AccountService.SetFrontendBaseUrlAsync(NavigationManager.BaseUri);
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
            NavigationManager_LocationChanged(null, null);
            await RefreshLayoutAsync();
        }

        private void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            PreviousUrl = CurrentUrl;
            CurrentUrl = NavigationManager.ToAbsoluteUri(NavigationManager.Uri)?.ToString().BeforeFirstOrWhole("?");
            StateHasChanged();
        }

        public async Task RefreshLayoutAsync()
        {
            var remoteStatus = NavigationManager.GetQueryString<string>("remoteStatus");
            var remoteMessage = NavigationManager.GetQueryString<string>("remoteMessage");

            if (remoteStatus != null && remoteMessage != null)
            {
                var refreshAuthenticationStateStr = NavigationManager.GetQueryString<string>("refreshAuthenticationState");
                var refreshAuthenticationState = !refreshAuthenticationStateStr.IsNullOrWhiteSpace() && Convert.ToBoolean(refreshAuthenticationStateStr);

                if (refreshAuthenticationState)
                {
                    var currentUrlWoQs = NavigationManager.Uri.BeforeFirstOrWholeIgnoreCase("?");
                    var qs = NavigationManager.Uri.QueryStringToDictionary();
                    qs.Remove("refreshAuthenticationState");
                    UserAuthStateProvider.StateChanged();
                    NavigationManager.NavigateTo($"{currentUrlWoQs}?{qs.ToQueryString()}", true);
                }
            }

            AuthenticatedUser = (await AccountService.GetAuthenticatedUserAsync())?.Result;
            await JsRuntime.InvokeVoidAsync("blazor_MainLayout_RefreshLayout");
            StateHasChanged();
        }

        public async Task PromptMessageAsync(PromptType status, string message, bool refresh = true) // don't refresh if called on initialized
        {
            PromptStatus = status;
            PromptMessage = message;
            await JsRuntime.InvokeVoidAsync("showAlerts");
            if (refresh)
                await RefreshLayoutAsync();
        }

        protected async Task BtnLogout_ClickAsync()
        {
            var logoutAsync = await AccountService.LogoutAsync();
            if (logoutAsync.IsError)
            {
                await PromptMessageAsync(PromptType.Error, logoutAsync.Message);
                return;
            }

            await PromptMessageAsync(PromptType.Success, logoutAsync.Message);
            UserAuthStateProvider.StateChanged();
        }
    }
}
