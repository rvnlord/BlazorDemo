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
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.Admin
{
    public class EditClaimBase : ComponentBase
    {
        private bool _firstRenderAfterInit;

        protected CustomDataAnnotationsValidator _validator;
        protected ButtonState _btnSaveClaimState;

        public AuthenticateUserVM AuthenticatedUser { get; set; } = AuthenticateUserVM.NotAuthenticated;
        public AdminEditClaimVM AdminEditClaimVM { get; set; } = new AdminEditClaimVM();
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

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        [Parameter]
        public string OriginalName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _btnSaveClaimState = ButtonState.Loading;
            EditContext = new EditContext(AdminEditClaimVM);
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
                await Main.PromptMessageAsync(PromptType.Error, "You are not Authorized to Access Edit Claim Admin Panel", false);
                _firstRenderAfterInit = true;
                return;
            }

            var claimResp = await AdminService.FindClaimByNameAsync(OriginalName);
            if (claimResp.IsError)
            {
                await Main.PromptMessageAsync(PromptType.Error, claimResp.Message);
                _firstRenderAfterInit = true;
                return;
            }

            var claim = claimResp.Result;
            if (claim == null)
            {
                await Main.PromptMessageAsync(PromptType.Error, $"There is no \"{OriginalName}\" Claim");
                _firstRenderAfterInit = true;
                return;
            }

            Mapper.Map(claim, AdminEditClaimVM);

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

            await JsRuntime.InvokeVoidAsync("blazor_Admin_EditClaim_AfterRender"); // Never call StatehasChanged in AfterRender
            _btnSaveClaimState = ButtonState.Enabled;
            StateHasChanged();
        }

        public async Task FormAdminEditClaim_ValidSubmitAsync()
        {
            _btnSaveClaimState = ButtonState.Loading;
            var addClaimResp = await AdminService.EditClaimAsync(AdminEditClaimVM);
            if (addClaimResp.IsError)
            {
                _btnSaveClaimState = ButtonState.Enabled;
                EditContext.AddValidationMessages(_validator.MessageStore, addClaimResp.ValidationMessages);
                await Main.PromptMessageAsync(PromptType.Error, addClaimResp.Message);
                return;
            }

            await Main.PromptMessageAsync(PromptType.Success, addClaimResp.Message);
            NavigationManager.NavigateTo("/admin/claims");
            await Task.CompletedTask;
        }

        protected async Task CbUser_CheckedAsync(string userName, bool isChecked)
        {
            if (!AdminEditClaimVM.Values.Any()) // since we are creating a new claim it has no values, since we don't care about values, only claim names, we can create a dummy value. Also this value cannot be null because _userManager will refuse to add Claim with null value
                AdminEditClaimVM.Values.Add(new AdminEditClaimValueVM { Value = "true" });

            if (isChecked)
                AdminEditClaimVM.Values.First().UserNames.Add(userName); // we don't use Claim Values, so we don't have to care which ones we take
            else
                AdminEditClaimVM.Values.First().UserNames.Remove(userName);

            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
