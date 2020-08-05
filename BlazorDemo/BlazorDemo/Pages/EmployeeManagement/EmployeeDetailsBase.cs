using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using BlazorDemo.Common.Utils.UtilClasses;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorDemo.Pages.EmployeeManagement
{
    public class EmployeeDetailsBase : ComponentBase
    {
        protected ButtonState _btnBackState;
        protected ButtonState _btnEditState;
        protected ButtonState _btnDeleteState;
        protected Point Coordinates { get; set; }
        protected string ToggleFooterButtonText { get; set; } = "Hide Footer";
        protected string FooterCssClass { get; set; }

        public AuthenticateUserVM AuthenticatedUser { get; set; }
        public bool IsAuthorized() => AuthenticatedUser?.IsAuthenticated == true && (AuthenticatedUser.HasRole("Admin") || AuthenticatedUser.HasClaim("View Employees"));
        public ConfirmationDialogBase DeleteConfirmation { get; set; }
        public Employee Employee { get; set; } = new Employee();

        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [CascadingParameter]
        public MainLayoutBase Main { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = (await AccountService.GetAuthenticatedUserAsync())?.Result;
            if (!IsAuthorized())
            {
                Main.PromptStatus = PromptType.Error;
                Main.PromptMessage = "To View Employee Details you need to be an \"Admin\" or you need to have \"View Users\" Claim";
                await Main.RefreshLayoutAsync();
                return;
            }

            Id ??= "1";
            Employee = await EmployeeService.GetEmployeeByIdAsync(int.Parse(Id));
        }

        protected void EmployeePhoto_MouseMove(MouseEventArgs e)
        {
            Coordinates = new Point(e.ClientX, e.ClientY);
        }

        protected void ToggleFooterButton_Click(MouseEventArgs obj)
        {
            if (ToggleFooterButtonText == "Hide Footer")
            {
                ToggleFooterButtonText = "Show Footer";
                FooterCssClass = "d-none";
            }
            else
            {
                ToggleFooterButtonText = "Hide Footer";
                FooterCssClass = null;
            }
        }

        protected void BtnBack_Click()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnBackState = ButtonState.Loading;
            NavigationManager.NavigateTo("/");
        }

        protected void BtnEdit_Click()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnEditState = ButtonState.Loading;
            NavigationManager.NavigateTo($"/editemployee/{Employee.Id}");
        }

        protected void BtnDelete_Click()
        {
            SetButtonStates(ButtonState.Disabled);
            _btnDeleteState = ButtonState.Loading;
            DeleteConfirmation.Show();
        }

        protected async Task BtnConfirmDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (isDeleteConfirmed)
            {
                await EmployeeService.DeleteEmployeeAsync(Employee.Id);
                NavigationManager.NavigateTo("/");
            }

            SetButtonStates(ButtonState.Enabled);
        }

        private void SetButtonStates(ButtonState state)
        {
            _btnBackState = state;
            _btnEditState = state;
            _btnDeleteState = state;
        }
    }
}
