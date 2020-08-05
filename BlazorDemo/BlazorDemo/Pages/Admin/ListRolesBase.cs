using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages.Admin
{
    public class ListRolesBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected AdminEditRoleVM _roleWaitingForDeleteConfirmation;
        protected Dictionary<Guid, ButtonState> _btnEditRoleStates = new Dictionary<Guid, ButtonState>();
        protected Dictionary<Guid, ButtonState> _btnDeleteRoleStates = new Dictionary<Guid, ButtonState>();
        protected ButtonState _btnAddRoleState;

        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public List<AdminEditRoleVM> RolesToEditByAdmin { get; set; } = new List<AdminEditRoleVM>();
        public ConfirmationDialogBase ConfirmationDialog_DeleteRole { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && AuthenticatedUser?.HasRole("Admin") == true;

        [Inject]
        public IAdminService AdminService { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = (await AccountService.GetAuthenticatedUserAsync())?.Result;
            if (!IsAuthorized())
            {
                await Main.PromptMessageAsync(PromptType.Error, "Roles can only be accessed by an Admin");
                _firstRenderAfterInit = true;
                return;
            }

            var rolesToEditbyAdminResponse = await AdminService.GetRolesAsync();
            if (rolesToEditbyAdminResponse.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, rolesToEditbyAdminResponse.Message);
                _firstRenderAfterInit = true;
                return;
            }

            RolesToEditByAdmin = Mapper.ProjectTo<AdminEditRoleVM>(rolesToEditbyAdminResponse.Result.AsQueryable()).ToList();
            _btnEditRoleStates.ReplaceAll(RolesToEditByAdmin.ToDictionary(c => c.Id, c => ButtonState.Loading));
            _btnDeleteRoleStates.ReplaceAll(RolesToEditByAdmin.ToDictionary(c => c.Id, c => ButtonState.Loading));
            _btnAddRoleState = ButtonState.Loading;

            _firstRenderAfterInit = true;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            SetButtonStates(ButtonState.Enabled);
            StateHasChanged();
        }

        public void BtnDeleteRole_ClickAsync(AdminEditRoleVM roleToDelete)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnDeleteRoleStates[roleToDelete.Id] = ButtonState.Loading;
            ConfirmationDialog_DeleteRole.Show($"Are you sure you want to delete Role \"{roleToDelete.Name}\"?");
            _roleWaitingForDeleteConfirmation = roleToDelete;
        }

        public async void BtnConfirmRoleDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (!isDeleteConfirmed)
            {
                SetButtonStates(ButtonState.Enabled);
                _roleWaitingForDeleteConfirmation = null;
                return;
            }

            var editRoleResponse = await AdminService.DeleteRoleAsync(_roleWaitingForDeleteConfirmation);
            var rolesToEditbyAdminResponse = await AdminService.GetRolesAsync();
            RolesToEditByAdmin = Mapper.ProjectTo<AdminEditRoleVM>(rolesToEditbyAdminResponse.Result.AsQueryable()).ToList();
            if (editRoleResponse.IsError || rolesToEditbyAdminResponse.IsError)
            {
                SetButtonStates(ButtonState.Enabled);
                await Main.PromptMessageAsync(PromptType.Error, editRoleResponse.Message ?? rolesToEditbyAdminResponse.Message);
                _roleWaitingForDeleteConfirmation = null;
                return;
            }

            SetButtonStates(ButtonState.Enabled);
            await Main.PromptMessageAsync(PromptType.Success, editRoleResponse.Message);
            _roleWaitingForDeleteConfirmation = null;
        }

        public void BtnEditRole_ClickAsync(AdminEditRoleVM roleToEdit)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnEditRoleStates[roleToEdit.Id] = ButtonState.Loading;
            NavigationManager.NavigateTo($"admin/editrole/{roleToEdit.Id}");
        }

        public void BtnAddRole_ClickAsync()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnAddRoleState = ButtonState.Loading;
            NavigationManager.NavigateTo("admin/addrole/");
        }

        private void SetButtonStates(ButtonState state)
        {
            _btnAddRoleState = state;
            foreach (var key in _btnEditRoleStates.Keys.ToArray())
                _btnEditRoleStates[key] = state;
            foreach (var key in _btnEditRoleStates.Keys.ToArray())
                _btnDeleteRoleStates[key] = state;
        }
    }
}
