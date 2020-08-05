using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Collections.Extensions;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Admin
{
    public class EditUserBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnSaveUserState;

        public AuthenticateUserVM AuthenticatedUser { get; set; } = AuthenticateUserVM.NotAuthenticated;
        public AdminEditUserVM AdminEditUserVM { get; set; } = new AdminEditUserVM();
        public EditContext EditContext { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && AuthenticatedUser.HasRole("Admin");
        public string AdminEditUserVMIdString { get; set; }
        public List<FindRoleVM> Roles { get; set; } = new List<FindRoleVM>();
        public List<FindClaimVM> Claims { get; set; } = new List<FindClaimVM>();

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IAdminService AdminService { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

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
            _btnSaveUserState = ButtonState.Loading;
            EditContext = new EditContext(AdminEditUserVM);
            var authUserResp = await AccountService.GetAuthenticatedUserAsync();
            if (authUserResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, authUserResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            AuthenticatedUser = authUserResp.Result;
            if (!IsAuthorized())
            {
                await Main.PromptMessageAsync(PromptType.Error, "You are not Authorized to Edit Users");
                _firstRenderAfterInit = true;
                return;
            }

            var userResp = await AdminService.FindUserByIdAsync(Id);
            if (userResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, userResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            var user = userResp.Result;
            if (user == null)
            {
                await Main.PromptMessageAsync(PromptType.Error, $"There is no User with the following id: {Id}");
                _firstRenderAfterInit = true;
                return;
            }

            Mapper.Map(user, AdminEditUserVM);
            AdminEditUserVM.ReturnUrl = "/Admin/Users";
            AdminEditUserVMIdString = AdminEditUserVM.Id.ToString();

            var rolesResp = await AdminService.GetRolesAsync();
            if (rolesResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, rolesResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            Roles = rolesResp.Result;

            var claimsResp = await AdminService.GetClaimsAsync();
            if (claimsResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, claimsResp.Message);
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

            await JsRuntime.InvokeVoidAsync("blazor_Admin_EditUser_AfterRender"); // Never call StatehasChanged in AfterRender
            _btnSaveUserState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormAdminEditUser_ValidSubmitAsync()
        {
            _btnSaveUserState = ButtonState.Loading;
            var editUserResponse = await AdminService.EditUserAsync(AdminEditUserVM);
            if (editUserResponse.IsError)
            {
                _btnSaveUserState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, editUserResponse.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, editUserResponse.Message);
                return;
            }

            await Main.PromptMessageAsync(PromptType.Success, editUserResponse.Message);
            NavigationManager.NavigateTo(AdminEditUserVM.ReturnUrl);
        }

        protected async Task CbRole_CheckedAsync(FindRoleVM role, bool isChecked)
        {
            if (isChecked)
                AdminEditUserVM.Roles.Add(role);
            else
                AdminEditUserVM.Roles.Remove(role);

            StateHasChanged();
            await Task.CompletedTask;
        }

        protected async Task CbClaim_CheckedAsync(FindClaimVM claim, bool isChecked)
        {
            if (isChecked)
                AdminEditUserVM.Claims.Add(claim); // for simplicity consider only claim type and omit the value, so we can take any (first) value, which is a key in this keys because values of the nested dict are UserNames
            else
                AdminEditUserVM.Claims.Remove(claim);

            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
