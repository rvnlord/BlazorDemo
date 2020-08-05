using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Services.Frontend;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Account
{
    [Authorize]
    public class EditBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnSaveEditsState;
        
        public EditUserVM EditUserVM { get; set; } = new EditUserVM();
        public EditContext EditContext { get; set; }
        public string EditUserVMIdString { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }
        public UserAuthenticationStateProvider UserAuthStateProvider => (UserAuthenticationStateProvider)AuthStateProvider;

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public IParametersService ParametersService { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnSaveEditsState = ButtonState.Loading;
            EditContext = new EditContext(EditUserVM);
            var authUserApiResponse = await AccountService.GetAuthenticatedUserAsync();
            if (authUserApiResponse.IsError || authUserApiResponse.Result == null || authUserApiResponse.Result.Id == default)
            {
                await Main.PromptMessageAsync(PromptType.Error, authUserApiResponse.Message ?? "Can't retrieve the Authenticated User from the API");
                _firstRenderAfterInit = true;
                return;
            }

            Mapper.Map(authUserApiResponse.Result, EditUserVM); // since we are Authorized here, it should always be there unless API is down
            EditUserVM.ReturnUrl = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).AbsoluteUri.BeforeFirstOrWhole("?");
            EditUserVMIdString = EditUserVM.Id.ToString();

            if (ParametersService.Parameters.VorN(nameof(EditUserVM)) is EditUserVM paramUser) // restore from param if we are refreshing manually after performing edits
            {
                Mapper.Map(paramUser, EditUserVM);
                ParametersService.Parameters.Remove(nameof(EditUserVM));
            }

            _firstRenderAfterInit = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Account_Edit_AfterRender");
            _btnSaveEditsState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormEdit_ValidSubmitAsync()
        {
            _btnSaveEditsState = ButtonState.Loading;
            var editResponse = await AccountService.EditAsync(EditUserVM);
            if (editResponse.IsError)
            {
                _btnSaveEditsState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, editResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, editResponse.Message);
                return;
            }

            Mapper.Map(editResponse.Result, EditUserVM); // rebinding user would break the existing references, for the mapping to work properly 'EditUserVM --> EditUserVM' is required, otherwise the new object will be created properly but the existing destination won't be updated
            ParametersService.Parameters[nameof(EditUserVM)] = EditUserVM;
            NavigationManager.NavigateTo(EditUserVM.ReturnUrl);
            await Main.PromptMessageAsync(PromptType.Success, editResponse.Message);
            UserAuthStateProvider.StateChanged(); // mandatory since we are logging user out if the email was changed
        }
    }
}
