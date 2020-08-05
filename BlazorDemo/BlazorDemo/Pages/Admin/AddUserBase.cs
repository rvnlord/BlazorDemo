using System;
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
    public class AddUserBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnAddNewUserState;

        public AuthenticateUserVM AuthenticatedUser { get; set; } = AuthenticateUserVM.NotAuthenticated;
        public AdminEditUserVM AdminAddUserVM { get; set; } = new AdminEditUserVM();
        public EditContext EditContext { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && AuthenticatedUser.HasRole("Admin");
        public List<FindRoleVM> Roles { get; set; } = new List<FindRoleVM>();
        public List<FindClaimVM> Claims { get; set; } = new List<FindClaimVM>();

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IAdminService AdminService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnAddNewUserState = ButtonState.Loading;
            EditContext = new EditContext(AdminAddUserVM);
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
                await Main.PromptMessageAsync(PromptType.Error, "You are not Authorized to Access Edit User Admin Panel", false);
                _firstRenderAfterInit = true;
                return;
            }

            AdminAddUserVM.ReturnUrl = "/Admin/Users";

            var rolesResp = await AdminService.GetRolesAsync();
            if (rolesResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, rolesResp.Message, false);
                _firstRenderAfterInit = true;
                return;
            }

            Roles = rolesResp.Result;

            var claimsResp = await AdminService.GetClaimsAsync();
            if (claimsResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, claimsResp.Message, false);
                _firstRenderAfterInit = true;
                return;
            }

            Claims = claimsResp.Result;

            _firstRenderAfterInit = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Admin_AddUser_AfterRender"); // Never call StatehasChanged in AfterRender
            _btnAddNewUserState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormAdminAddUser_ValidSubmitAsync()
        {
            _btnAddNewUserState = ButtonState.Loading;
            var editUserResponse = await AdminService.AddUserAsync(AdminAddUserVM);
            if (editUserResponse.IsError)
            {
                _btnAddNewUserState = ButtonState.Enabled;
                await Main.PromptMessageAsync(PromptType.Error, editUserResponse.Message);
                return;
            }

            await Main.PromptMessageAsync(PromptType.Success, editUserResponse.Message);
            NavigationManager.NavigateTo(AdminAddUserVM.ReturnUrl);
        }

        protected async Task CbRole_CheckedAsync(FindRoleVM role, bool isChecked)
        {
            if (isChecked)
                AdminAddUserVM.Roles.Add(role);
            else
                AdminAddUserVM.Roles.Remove(role);

            StateHasChanged();
            await Task.CompletedTask;
        }

        protected async Task CbClaim_CheckedAsync(FindClaimVM claim, bool isChecked)
        {
            if (isChecked)
                AdminAddUserVM.Claims.Add(claim);
            else
                AdminAddUserVM.Claims.Remove(claim);

            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
