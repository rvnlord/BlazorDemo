using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Utils;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Account
{
    public class ResendEmailConfirmationBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnResendEmailConfirmationState;
      
        public ResendEmailConfirmationUserVM ResendEmailConfirmationUserVM { get; set; } = new ResendEmailConfirmationUserVM();
        public string ReturnUrl { get; set; }
        public EditContext EditContext { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

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
            _btnResendEmailConfirmationState = ButtonState.Loading;
            EditContext = new EditContext(ResendEmailConfirmationUserVM);
            ResendEmailConfirmationUserVM.ReturnUrl = NavigationManager.GetQueryString<string>("returnUrl")?.HtmlDecode() ?? "/account/login"; // we can arrive here from register, login or if user types this address but we want to redirect to confirm email from here regardless
            _firstRenderAfterInit = true;
            await Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Account_ResendEmailConfirmation_AfterRender");
            _btnResendEmailConfirmationState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormResendEmailConfirmation_ValidSubmitAsync()
        {
            _btnResendEmailConfirmationState = ButtonState.Loading;
            var resendEmailConfirmationResponse = await AccountService.ResendEmailConfirmationAsync(ResendEmailConfirmationUserVM);
            if (resendEmailConfirmationResponse.IsError)
            {
                _btnResendEmailConfirmationState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, resendEmailConfirmationResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, resendEmailConfirmationResponse.Message);
                return;
            }

            NavigationManager.NavigateTo($"/Account/ConfirmEmail/?email={ResendEmailConfirmationUserVM.Email}&returnUrl={ResendEmailConfirmationUserVM.ReturnUrl?.HtmlEncode()}");
            await Main.PromptMessageAsync(PromptType.Success, resendEmailConfirmationResponse.Message);
        }
    }
}
