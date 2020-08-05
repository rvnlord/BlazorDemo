using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    public class EditRoleBase : ComponentBase
    { 
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnSaveRoleState;

        public AuthenticateUserVM AuthenticatedUser { get; set; } = AuthenticateUserVM.NotAuthenticated;
        public AdminEditRoleVM AdminEditRoleVM { get; set; } = new AdminEditRoleVM();
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

        [Inject]
        public IMapper Mapper { get; set; }

        [Parameter]
        public Guid Id { get; set; }
        public string AdminEditRoleVMIdString { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnSaveRoleState = ButtonState.Loading;
            EditContext = new EditContext(AdminEditRoleVM);
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

            var roleResp = await AdminService.FindRoleByIdAsync(Id);
            if (roleResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, roleResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            var role = roleResp.Result;
            if (role == null)
            {
                await Main.PromptMessageAsync(PromptType.Error, $"There is no Role with the following id: {Id}");
                _firstRenderAfterInit = true;
                return;
            }

            Mapper.Map(role, AdminEditRoleVM);
            AdminEditRoleVMIdString = AdminEditRoleVM.Id.ToString();

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

            await JsRuntime.InvokeVoidAsync("blazor_Admin_EditRole_AfterRender"); // Never call StatehasChanged in AfterRender
            _btnSaveRoleState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormAdminEditRole_ValidSubmitAsync()
        {
            _btnSaveRoleState = ButtonState.Loading;
            var editRoleResp = await AdminService.EditRoleAsync(AdminEditRoleVM);
            if (editRoleResp.IsError)
            {
                _btnSaveRoleState = ButtonState.Enabled;
                await Main.PromptMessageAsync(PromptType.Error, editRoleResp.Message);
                return;
            }

            await Main.PromptMessageAsync(PromptType.Success, editRoleResp.Message);
            NavigationManager.NavigateTo("/admin/roles");
            await Task.CompletedTask;
        }

        protected async Task CbUser_CheckedAsync(string userName, bool isChecked)
        {
            if (isChecked)
                AdminEditRoleVM.UserNames.Add(userName);
            else
                AdminEditRoleVM.UserNames.Remove(userName);

            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
