using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Account
{
    public class RegisterBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnRegisterState;
        private bool _isRendered;

        public RegisterUserVM RegisterUserVM { get; set; } = new RegisterUserVM();

        public EditContext EditContext { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider)AuthStateProvider;

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnRegisterState = ButtonState.Loading;
            RegisterUserVM.ReturnUrl = NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode() ?? "/account/login?keepPrompt=true";
            EditContext = new EditContext(RegisterUserVM);
            _firstRenderAfterInit = true;
            await Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;
            _isRendered = true;

            await JsRuntime.InvokeVoidAsync("blazor_Account_Register_AfterRender");
            _btnRegisterState = ButtonState.Enabled;
            StateHasChanged();
        }

        protected async Task FormRegister_ValidSubmitAsync()
        {
            _btnRegisterState = ButtonState.Loading;
            var registrationResult = await AccountService.RegisterAsync(RegisterUserVM);
            if (registrationResult.IsError)
            {
                _btnRegisterState = ButtonState.Enabled;
                if (registrationResult.Result?.ReturnUrl != null && RegisterUserVM.ReturnUrl != registrationResult.Result.ReturnUrl)
                    NavigationManager.NavigateTo(registrationResult.Result.ReturnUrl); // redirect to `ResendEmailConfirmation` on successful registration but when email couldn't be deployed
                EditContext.AddValidationMessages(_validator.MessageStore, registrationResult.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, registrationResult.Message);
                return;
            }

            var registereduser = registrationResult.Result;
            NavigationManager.NavigateTo(registereduser.ReturnUrl);
            await Main.PromptMessageAsync(PromptType.Success, registrationResult.Message);
            UserAuthStateProvider.StateChanged();
        }

    }
}