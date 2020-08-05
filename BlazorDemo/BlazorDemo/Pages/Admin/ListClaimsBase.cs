using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Extensions.Collections;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages.Admin
{
    public class ListClaimsBase : ComponentBase
    {
        private bool _firstRenderAfterInit;
        
        protected AdminEditClaimVM _claimWaitingForDeleteConfirmation;
        protected Dictionary<string, ButtonState> _btnEditClaimStates = new Dictionary<string, ButtonState>();
        protected Dictionary<string, ButtonState> _btnDeleteClaimStates = new Dictionary<string, ButtonState>();
        protected ButtonState _btnAddClaimState;

        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public List<AdminEditClaimVM> ClaimsToEditByAdmin { get; set; } = new List<AdminEditClaimVM>();
        public ConfirmationDialogBase ConfirmationDialog_DeleteClaim { get; set; }
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
                await Main.PromptMessageAsync(PromptType.Error, "Claims can only be accessed by an Admin");
                _firstRenderAfterInit = true;
                return;
            }

            var claimsToEditbyAdminResponse = await AdminService.GetClaimsAsync();
            if (claimsToEditbyAdminResponse.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, claimsToEditbyAdminResponse.Message);
                _firstRenderAfterInit = true;
                return;
            }

            ClaimsToEditByAdmin = Mapper.ProjectTo<AdminEditClaimVM>(claimsToEditbyAdminResponse.Result.Where(c => !c.Name.EqualsIgnoreCase("Email")).AsQueryable()).ToList();
            _btnEditClaimStates.ReplaceAll(ClaimsToEditByAdmin.ToDictionary(c => c.Name, c => ButtonState.Loading));
            _btnDeleteClaimStates.ReplaceAll(ClaimsToEditByAdmin.ToDictionary(c => c.Name, c => ButtonState.Loading));
            _btnAddClaimState = ButtonState.Loading;

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

        public void BtnDeleteClaim_ClickAsync(AdminEditClaimVM claimToDelete)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnDeleteClaimStates[claimToDelete.Name] = ButtonState.Loading;
            ConfirmationDialog_DeleteClaim.Show($"Are you sure you want to delete Claim \"{claimToDelete.Name}\"?");;
            _claimWaitingForDeleteConfirmation = claimToDelete;
        }

        public async void BtnConfirmClaimDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (!isDeleteConfirmed)
            {
                SetButtonStates(ButtonState.Enabled);
                _claimWaitingForDeleteConfirmation = null;
                return;
            }

            var editClaimResponse = await AdminService.DeleteClaimAsync(_claimWaitingForDeleteConfirmation);
            var claimsToEditbyAdminResponse = await AdminService.GetClaimsAsync();
            ClaimsToEditByAdmin = Mapper.ProjectTo<AdminEditClaimVM>(claimsToEditbyAdminResponse.Result.Where(c => !c.Name.EqualsIgnoreCase("Email")).AsQueryable()).ToList();
            if (editClaimResponse.IsError || claimsToEditbyAdminResponse.IsError)
            {
                SetButtonStates(ButtonState.Enabled);
                await Main.PromptMessageAsync(PromptType.Error, editClaimResponse.Message ?? claimsToEditbyAdminResponse.Message);
                _claimWaitingForDeleteConfirmation = null;
                return;
            }

            SetButtonStates(ButtonState.Enabled);
            await Main.PromptMessageAsync(PromptType.Success, editClaimResponse.Message);
            _claimWaitingForDeleteConfirmation = null;
        }

        public void BtnEditClaim_ClickAsync(AdminEditClaimVM claimToEdit)
        {
            SetButtonStates(ButtonState.Disabled);
            _btnEditClaimStates[claimToEdit.Name] = ButtonState.Loading;
            NavigationManager.NavigateTo($"admin/editclaim/{claimToEdit.Name}");
        }

        public void BtnAddClaim_ClickAsync()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnAddClaimState = ButtonState.Loading;
            NavigationManager.NavigateTo("admin/addclaim/");
        }

        private void SetButtonStates(ButtonState state)
        {
            _btnAddClaimState = state;
            foreach (var key in _btnEditClaimStates.Keys.ToArray())
                _btnEditClaimStates[key] = state;
            foreach (var key in _btnEditClaimStates.Keys.ToArray())
                _btnDeleteClaimStates[key] = state;
        }
    }
}
