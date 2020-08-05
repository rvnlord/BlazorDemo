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
    public class ResetPasswordBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnChangePasswordState;

        public ResetPasswordUserVM ResetPasswordUserVM { get; set; } = new ResetPasswordUserVM();
        public EditContext EditContext { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider)AuthStateProvider;

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnChangePasswordState = ButtonState.Loading;
            EditContext = new EditContext(ResetPasswordUserVM);
            ResetPasswordUserVM.ReturnUrl = NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode() ?? "/account/login";
            var email = NavigationManager.GetQueryString<string>("email");
            var code = NavigationManager.GetQueryString<string>("code");
            ResetPasswordUserVM.Email = email;
            ResetPasswordUserVM.ResetPasswordCode = code;
            var userResponse = await AccountService.FindUserByEmailAsync(email);
            if (!userResponse.IsError && userResponse.Result != null)
                ResetPasswordUserVM.UserName = userResponse.Result.UserName;
            _firstRenderAfterInit = true;
            await Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Account_ResetPassword_AfterRender");
            _btnChangePasswordState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormResetPassword_ValidSubmitAsync()
        {
            if (ResetPasswordUserVM.Email.IsNullOrWhiteSpace() || ResetPasswordUserVM.ResetPasswordCode.IsNullOrWhiteSpace())
                return; // return silently because user chose to provide code manually

            _btnChangePasswordState = ButtonState.Loading;
            var resetPasswordResponse = await AccountService.ResetPasswordAsync(ResetPasswordUserVM);
            if (resetPasswordResponse.IsError)
            {
                _btnChangePasswordState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, resetPasswordResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, resetPasswordResponse.Message);
                return;
            }

            NavigationManager.NavigateTo($"/Account/Login?returnUrl={ResetPasswordUserVM.ReturnUrl?.HtmlEncode()}");
            await Main.PromptMessageAsync(PromptType.Success, resetPasswordResponse.Message);
        }
    }
}
