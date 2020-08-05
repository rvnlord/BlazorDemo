using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages.EmployeeManagement
{
    public class EmployeeListBase : ComponentBase
    {
        private Employee _employeeWaitingForDelete;
        private bool _firstRenderAfterInit;

        public Dictionary<int, ButtonState> BtnEmployeeDetailsStates { get; } = new Dictionary<int, ButtonState>();
        public Dictionary<int, ButtonState> BtnEditEmployeeStates { get; } = new Dictionary<int, ButtonState>();
        public Dictionary<int, ButtonState> BtnDeleteEmployeeStates { get; } = new Dictionary<int, ButtonState>();
        public IEnumerable<Employee> Employees { get; set; }
        public bool ShowFooter { get; set; } = true;
        public int SelectedEmployeesCount { get; set; }
        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && (AuthenticatedUser.HasRole("Admin") || AuthenticatedUser.HasClaim("View Employees"));
        public ConfirmationDialogBase DeleteConfirmation { get; set; }

        [Inject] 
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = (await AccountService.GetAuthenticatedUserAsync())?.Result; // this sure needs the auth also oon the API side but I leave the EmployeeManager as is for clarity as it shows different 'HTTP' methods, custom Backend auth is already demonstrated with User Management
            if (!IsAuthorized())
            {
                Main.PromptStatus = PromptType.Error;
                Main.PromptMessage = "To access the List of Employees you need to be an \"Admin\" or you need to have \"View Users\" Claim";
                await Main.RefreshLayoutAsync();
                return;
            }

            await ReloadEmployeeList();

            _firstRenderAfterInit = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRenderAfterInit)
                return;

            _firstRenderAfterInit = false;

            await JsRuntime.InvokeVoidAsync("blazor_Employee_EmployeeList_AfterRender");
            StateHasChanged();
        }

        protected void Employee_AddedToList(int id)
        {
            BtnEmployeeDetailsStates[id] = ButtonState.Enabled;
            BtnEditEmployeeStates[id] = ButtonState.Enabled;
            BtnDeleteEmployeeStates[id] = ButtonState.Enabled;
        }

        protected void Employee_Selected(bool isEmployeeSelected)
        {
            SelectedEmployeesCount = isEmployeeSelected ? SelectedEmployeesCount + 1 : SelectedEmployeesCount - 1;
        }

        protected void Employee_DetailsDisplayed(int id)
        {
            SetButtonStates(ButtonState.Disabled);
            BtnEmployeeDetailsStates[id] = ButtonState.Loading;
            StateHasChanged();
        }

        protected void Employee_EditDisplayed(int id)
        {
            SetButtonStates(ButtonState.Disabled);
            BtnEditEmployeeStates[id] = ButtonState.Loading;
            StateHasChanged();
        }

        protected void Employee_DeletePromptOpened(Employee employeeToDelete)
        {
            SetButtonStates(ButtonState.Disabled);
            BtnDeleteEmployeeStates[employeeToDelete.Id] = ButtonState.Loading;
            DeleteConfirmation.Show($"Are you sure you want to delete \"{employeeToDelete.FirstName}\"?");
            _employeeWaitingForDelete = employeeToDelete;
            StateHasChanged();
        }

        protected async Task BtnConfirmDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (isDeleteConfirmed)
            {
                await EmployeeService.DeleteEmployeeAsync(_employeeWaitingForDelete.Id);
                Main.PromptStatus = PromptType.Success;
                Main.PromptMessage = $"Employee (id = {_employeeWaitingForDelete.Id}): \"{_employeeWaitingForDelete.FirstName} {_employeeWaitingForDelete.LastName}\" has been deleted";
                await ReloadEmployeeList();
            }

            _employeeWaitingForDelete = null;
            SetButtonStates(ButtonState.Enabled);
            StateHasChanged();
        }

        private async Task ReloadEmployeeList()
        {
            try
            {
                Employees = await EmployeeService.GetEmployeesAsync();
                if (Main.PromptStatus == PromptType.Error)
                {
                    Main.PromptStatus = PromptType.Success;
                    Main.PromptMessage = null;
                }
            }
            catch (Exception)
            {
                Employees = new List<Employee>();
                Main.PromptStatus = PromptType.Error;
                Main.PromptMessage = $"There was a problem while retrieving data through {nameof(EmployeeService)}";
            }
            finally
            {
                StateHasChanged();
            }
        }

        private void SetButtonStates(ButtonState state)
        {
            foreach (var key in BtnEmployeeDetailsStates.Keys.ToArray())
                BtnEmployeeDetailsStates[key] = state;
            foreach (var key in BtnEditEmployeeStates.Keys.ToArray())
                BtnEditEmployeeStates[key] = state;
            foreach (var key in BtnDeleteEmployeeStates.Keys.ToArray())
                BtnDeleteEmployeeStates[key] = state;
        }
    }
}
