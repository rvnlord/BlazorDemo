using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Admin
{
    public class AddRoleBase : ComponentBase
    { 
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnAddNewRoleState;

        public AuthenticateUserVM AuthenticatedUser { get; set; } = AuthenticateUserVM.NotAuthenticated;
        public AdminEditRoleVM AdminAddRoleVM { get; set; } = new AdminEditRoleVM();
        public EditContext EditContext { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && AuthenticatedUser.HasRole("Admin");
        public List<FindUserVM> Users { get; set; } = new List<FindUserVM>();

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IAdminService AdminService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnAddNewRoleState = ButtonState.Loading;
            EditContext = new EditContext(AdminAddRoleVM);
            var authUserResp = await AccountService.GetAuthenticatedUserAsync();
            if (authUserResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, authUserResp.Message, false);
                _firstRenderAfterInit = true;
                return;
            }

            AuthenticatedUser = authUserResp.Result;
            if (!IsAuthorized())
            {
                await Main.PromptMessageAsync(PromptType.Error, "You are not Authorized to Access Edit Role Admin Panel", false);
                _firstRenderAfterInit = true;
                return;
            }

            var usersResp = await AdminService.GetAllUsersAsync();
            if (usersResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, usersResp.Message, false);
                _firstRenderAfterInit = true;
                return;
            }

            Users = usersResp.Result;

            _firstRenderAfterInit = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Admin_AddRole_AfterRender"); // Never call StatehasChanged in AfterRender
            _btnAddNewRoleState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormAdminAddRole_ValidSubmitAsync()
        {
            _btnAddNewRoleState = ButtonState.Loading;
            var addRoleResp = await AdminService.AddRoleAsync(AdminAddRoleVM);
            if (addRoleResp.IsError)
            {
                _btnAddNewRoleState = ButtonState.Enabled;
                await Main.PromptMessageAsync(PromptType.Error, addRoleResp.Message);
                return;
            }

            await Main.PromptMessageAsync(PromptType.Success, addRoleResp.Message);
            NavigationManager.NavigateTo("/admin/roles");
            await Task.CompletedTask;
        }

        protected async Task CbUser_CheckedAsync(string userName, bool isChecked)
        {
            if (isChecked)
                AdminAddRoleVM.UserNames.Add(userName);
            else
                AdminAddRoleVM.UserNames.Remove(userName);

            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
