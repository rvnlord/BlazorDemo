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
    public class ListUsersBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected AdminEditUserVM _userWaitingForDeleteConfirmation;
        protected Dictionary<Guid, ButtonState> _btnEditUserStates = new Dictionary<Guid, ButtonState>();
        protected Dictionary<Guid, ButtonState> _btnDeleteUserStates = new Dictionary<Guid, ButtonState>();
        protected ButtonState _btnAddUserState;

        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public List<FindUserVM> Users { get; set; } = new List<FindUserVM>();
        public ConfirmationDialogBase ConfirmationDialog_DeleteUser { get; set; }
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
                await Main.PromptMessageAsync(PromptType.Error, "Users can only be accessed by an Admin");
                _firstRenderAfterInit = true;
                return;
            }

            var foundUsersResp = await AdminService.GetAllUsersAsync();
            if (foundUsersResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, foundUsersResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            Users = foundUsersResp.Result;
            _btnEditUserStates.ReplaceAll(Users.ToDictionary(c => c.Id, c => ButtonState.Loading));
            _btnDeleteUserStates.ReplaceAll(Users.ToDictionary(c => c.Id, c => ButtonState.Loading));
            _btnAddUserState = ButtonState.Loading;

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

        public void BtnDeleteUser_ClickAsync(FindUserVM userToDelete)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnDeleteUserStates[userToDelete.Id] = ButtonState.Loading;
            ConfirmationDialog_DeleteUser.Show($"Are you sure you want to delete User \"{userToDelete.UserName}\"?");
            _userWaitingForDeleteConfirmation = Mapper.Map(userToDelete, new AdminEditUserVM());
        }

        public async void BtnConfirmUserDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (!isDeleteConfirmed)
            {
                SetButtonStates(ButtonState.Enabled);
                _userWaitingForDeleteConfirmation = null;
                StateHasChanged();
                return;
            }

            var editResponse = await AdminService.DeleteUserAsync(_userWaitingForDeleteConfirmation);
            var usersToEditbyAdminResponse = await AdminService.GetAllUsersAsync();
            Users = usersToEditbyAdminResponse.Result;
            if (editResponse.IsError || usersToEditbyAdminResponse.IsError)
            {
                SetButtonStates(ButtonState.Enabled);
                await Main.PromptMessageAsync(PromptType.Error, editResponse.Message ?? usersToEditbyAdminResponse.Message);
                _userWaitingForDeleteConfirmation = null;
                StateHasChanged();
                return;
            }

            SetButtonStates(ButtonState.Enabled);
            await Main.PromptMessageAsync(PromptType.Success, editResponse.Message);
            _userWaitingForDeleteConfirmation = null;
            StateHasChanged();
        }

        public void BtnEditUser_ClickAsync(FindUserVM userToEdit)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnEditUserStates[userToEdit.Id] = ButtonState.Loading;
            NavigationManager.NavigateTo($"admin/edituser/{userToEdit.Id}");
        }

        public void BtnAddUser_ClickAsync()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnAddUserState = ButtonState.Loading;
            NavigationManager.NavigateTo("admin/adduser/");
        }

        private void SetButtonStates(ButtonState state)
        {
            _btnAddUserState = state;
            foreach (var key in _btnEditUserStates.Keys.ToArray())
                _btnEditUserStates[key] = state;
            foreach (var key in _btnEditUserStates.Keys.ToArray())
                _btnDeleteUserStates[key] = state;
        }
    }
}
