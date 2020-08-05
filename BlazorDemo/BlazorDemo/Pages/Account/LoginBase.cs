using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using static BlazorDemo.Common.Utils.ConfigUtils;

namespace BlazorDemo.Pages.Account
{
    public class LoginBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected bool _isRendered;
        protected ButtonState _btnLoginState;
        protected Dictionary<string, ButtonState> _btnExternalLoginStates = new Dictionary<string, ButtonState>();
        protected ButtonState[] _btnStates => _btnExternalLoginStates?.Values.Prepend(_btnLoginState).ToArray() ?? new ButtonState[]{};

        public LoginUserVM LoginUserVM { get; set; } = new LoginUserVM();
        public EditContext EditContext { get; set; }
        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true;
        public CustomButtonBase BtnLogin { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider)AuthStateProvider;

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnLoginState = ButtonState.Loading;
            EditContext = new EditContext(LoginUserVM);
            LoginUserVM.ReturnUrl = $"{NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode()?.BeforeFirstOrWhole("?")}?keepPrompt=true";
            LoginUserVM.ExternalLogins = (await AccountService.GetExternalAuthenticationSchemesAsync()).Result; // methods executed on load events (initialised, afterrender, parametersset) can't raise `AuthenticationStateChanged` Event because it would cause an infinite loop when the Control State changes
            _btnExternalLoginStates.ReplaceAll(LoginUserVM.ExternalLogins.ToDictionary(l => l.Name, l => ButtonState.Loading));
            AuthenticatedUser = (await AccountService.GetAuthenticatedUserAsync()).Result;
            var queryUser = NavigationManager.GetQueryString<string>("user")?.Base64SafeUrlToUTF8OrNull()?.JsonDeserializeOrNull()?.To<LoginUserVM>();

            if (!IsAuthorized()) // try to authorize with what is present in queryStrings, possibly from an external provider
            {
                var remoteStatus = NavigationManager.GetQueryString<string>("remoteStatus")?.ToEnumN<PromptType>();
                var remoteMessage = NavigationManager.GetQueryString<string>("remoteMessage");

                if (remoteStatus == PromptType.Error)
                {
                    await Main.PromptMessageAsync(remoteStatus.ToEnum<PromptType>(), remoteMessage?.Base64SafeUrlToUTF8OrNull() ?? "Unable to Log In with an External provider");
                    _firstRenderAfterInit = true;
                    return;
                }

                if (queryUser != null && !IsAuthorized())
                {
                    SetButtonsState(ButtonState.Disabled);
                    _btnExternalLoginStates[queryUser.ExternalProvider] = ButtonState.Loading;
                    StateHasChanged();

                    await ExternalLoginAuthorizeAsync(queryUser);
                    _firstRenderAfterInit = true;
                    return;
                }

                _firstRenderAfterInit = true;
                return;
            }

            var keepPrompt = NavigationManager.GetQueryString<bool?>("keepPrompt") ?? false;
            if (queryUser == null && !keepPrompt) // if we have just logged in with an external provider then we want to leave the provider message visible, otherwise we let the user know he is logged in
                await Main.PromptMessageAsync(PromptType.Success, $"You are logged in as \"{AuthenticatedUser.UserName}\"");
            _firstRenderAfterInit = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("blazor_Account_Login_AfterRender"); // this has to be called regardless and before too so it doesnt flicker to valid state when external login calls back

            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false; // this allows us to execute after render only once when the razor markup is initialized and it allows us to call statehaschanged from this very event without triggering an loop
            _isRendered = true;

            await JsRuntime.InvokeVoidAsync("blazor_Account_Login_AfterRender");
            SetButtonsState(ButtonState.Enabled);
            StateHasChanged();
        }

        private async Task ExternalLoginAuthorizeAsync(LoginUserVM queryUser)
        {
            SetButtonsState(ButtonState.Disabled);
            _btnExternalLoginStates[queryUser.ExternalProvider] = ButtonState.Loading;

            var externalSchemes = LoginUserVM.ExternalLogins.ToList(); // would be overwritten by automapper;
            Mapper.Map(queryUser, LoginUserVM);
            LoginUserVM.ExternalLogins = externalSchemes;
            var externalLoginResult = await AccountService.ExternalLoginAuthorizeAsync(LoginUserVM);
            if (externalLoginResult.IsError)
            {
                SetButtonsState(ButtonState.Enabled);
                EditContext.AddValidationMessages(_validator.MessageStore, externalLoginResult.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, externalLoginResult.Message);
                return;
            }

            NavigationManager.NavigateTo(externalLoginResult.Result.ReturnUrl);
            await Main.PromptMessageAsync(PromptType.Success, externalLoginResult.Message);
            UserAuthStateProvider.StateChanged();
        }

        protected async Task FormLogin_ValidSubmitAsync()
        {
           SetButtonsState(ButtonState.Disabled);
            _btnLoginState = ButtonState.Loading;

            var loginResult = await AccountService.LoginAsync(LoginUserVM);
            if (loginResult.IsError)
            {
                SetButtonsState(ButtonState.Enabled);
                EditContext.AddValidationMessages(_validator.MessageStore, loginResult.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, loginResult.Message);
                return;
            }

            NavigationManager.NavigateTo(loginResult.Result.ReturnUrl);
            await Main.PromptMessageAsync(PromptType.Success, loginResult.Message);
            UserAuthStateProvider.StateChanged();
        }

        protected void ButtonExternalLogin_Click(MouseEventArgs e, string provider)
        {
            SetButtonsState(ButtonState.Disabled);
            _btnExternalLoginStates[provider] = ButtonState.Loading;

            LoginUserVM.ExternalProvider = provider;
            var url = $"{ApiBaseUrl}/api/account/externallogin";
            var qs = new Dictionary<string, string>
            {
                ["provider"] = LoginUserVM.ExternalProvider,
                ["returnUrl"] = LoginUserVM.ReturnUrl.HtmlEncode(),
                ["rememberMe"] = LoginUserVM.RememberMe.ToString().ToLowerInvariant()
            };
            NavigationManager.NavigateTo($"{url}?{qs.ToQueryString()}", true);
        }

        private void SetButtonsState(ButtonState state)
        {
            _btnLoginState = state;
            foreach (var key in _btnExternalLoginStates.Keys.ToArray())
                _btnExternalLoginStates[key] = state;
        }
    }
}
