using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
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
    public class ConfirmEmailBase : ComponentBase
    {
        private bool _firstRenderAfterinit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnConfirmEmailState;

        public bool IsEmailConfirmed { get; set; }
        public ConfirmUserVM ConfirmUserVM { get; set; } = new ConfirmUserVM();
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
            _btnConfirmEmailState = ButtonState.Loading;
            EditContext = new EditContext(ConfirmUserVM);
            ConfirmUserVM.ReturnUrl = NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode() ?? "/account/login";
            var email = NavigationManager.GetQueryString<string>("email");
            var code = NavigationManager.GetQueryString<string>("code");
            ConfirmUserVM.Email = email;
            ConfirmUserVM.ConfirmationCode = code;
            _firstRenderAfterinit = true;
            await Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterinit)
                return;

            _firstRenderAfterinit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Account_ConfirmEmail_AfterRender");

            if (!ConfirmUserVM.Email.IsNullOrWhiteSpace() && !ConfirmUserVM.ConfirmationCode.IsNullOrWhiteSpace())
            {
                await ConfirmEmailAsync();
                _btnConfirmEmailState = ButtonState.Enabled;
                return; // return silently because user chose to provide code manually
            }
           
            _btnConfirmEmailState = ButtonState.Enabled;
            StateHasChanged();
        }

        protected async Task FormConfirmEmail_ValidSubmitAsync()
        {
            await ConfirmEmailAsync();
        }

        private async Task ConfirmEmailAsync()
        {
            _btnConfirmEmailState = ButtonState.Loading;
            var emailConfirmationResponse = await AccountService.ConfirmEmailAsync(ConfirmUserVM.Email, ConfirmUserVM.ConfirmationCode);
            if (emailConfirmationResponse.IsError)
            {
                _btnConfirmEmailState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, emailConfirmationResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, emailConfirmationResponse.Message); // can't update state on afterrender because it would cause an infinite loop
                return;
            }

            var confirmedUser = emailConfirmationResponse.Result;
            ConfirmUserVM.UserName = confirmedUser.UserName;
            IsEmailConfirmed = true;
            NavigationManager.NavigateTo($"/Account/Login/?returnUrl={ConfirmUserVM.ReturnUrl?.HtmlEncode()}");
            await Main.PromptMessageAsync(PromptType.Success, $"Email for user: \"{confirmedUser.UserName}\" has been confirmed successfully", false); // can't update state on afterrender because it would cause an infinite loop

        } // https://localhost:44396/Account/ConfirmEmail?email=rvnlord@gmail.com&code=xxx
    }
}
