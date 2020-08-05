using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Account
{
    public class ForgotPasswordBase : ComponentBase
    {
        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnForgotPasswordState;
        private bool _firstRenderAfterInit;

        public ForgotPasswordUserVM ForgotPasswordUserVM { get; set; } = new ForgotPasswordUserVM();
        public EditContext EditContext { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider)AuthStateProvider;

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IParametersService ParametersService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnForgotPasswordState = ButtonState.Loading;
            EditContext = new EditContext(ForgotPasswordUserVM);
            ForgotPasswordUserVM.ReturnUrl = NavigationManager.ToAbsoluteUri(NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode() ?? "/account/login").AbsoluteUri.BeforeFirstOrWhole("?");
            _firstRenderAfterInit = true;
            await Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Account_ForgotPassword_AfterRender");
            _btnForgotPasswordState = ButtonState.Enabled;
            StateHasChanged();
        }

        protected async Task FormForgotPassword_ValidSubmitAsync()
        {
            _btnForgotPasswordState = ButtonState.Loading;
            var forgotPasswordResponse = await AccountService.ForgotPasswordAsync(ForgotPasswordUserVM);
            if (forgotPasswordResponse.IsError)
            {
                _btnForgotPasswordState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, forgotPasswordResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, forgotPasswordResponse.Message);
                return;
            }

            var forgotPasswordUser = forgotPasswordResponse.Result;
            await Main.PromptMessageAsync(PromptType.Success, $"Reset Password link sent to: \"{forgotPasswordUser.Email}\"");
            NavigationManager.NavigateTo($"/Account/ResetPassword?email={forgotPasswordUser.Email}&returnUrl={forgotPasswordUser.ReturnUrl?.HtmlEncode()}");
        }
    }
}
