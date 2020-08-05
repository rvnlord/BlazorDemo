using System.Threading.Tasks;
using BlazorDemo.Common.Components;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages.EmployeeManagement
{
    public class DisplayEmployeeBase : ComponentBase
    {
        protected string SelectEmployeeCheckBoxId { get; set; }
        protected bool IsEmployeeSelected { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public EventCallback<int> OnEmployeeAddedToList { get; set; }

        [Parameter]
        public EventCallback<bool> OnEmployeeSelection { get; set; }

        [Parameter]
        public EventCallback<Employee> OnEmployeeDeletePromptOpen { get; set; }

        [Parameter]
        public EventCallback<int> OnEmployeeDetailsDisplayed { get; set; }

        [Parameter]
        public EventCallback<int> OnEmployeeEditDisplayed { get; set; }

        [Parameter]
        public Employee Employee { get; set; }

        [Parameter]
        public bool ShowFooter { get; set; }

        [Parameter]
        public EmployeeListBase EmployeeListContext { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SelectEmployeeCheckBoxId = $"cbSelect{Employee.FirstName}{@Employee.LastName}";
            await OnEmployeeAddedToList.InvokeAsync(Employee.Id);
        }

        protected async Task CbSelectEmployee_ChangedAsync(ChangeEventArgs e)
        {
            IsEmployeeSelected = (bool)e.Value;
            await OnEmployeeSelection.InvokeAsync(IsEmployeeSelected);
        }

        protected async Task BtnDelete_ClickAsync()
        {
            await OnEmployeeDeletePromptOpen.InvokeAsync(Employee);
        }

        protected async Task BtnEmployeeDetails_ClickAsync()
        {
            await OnEmployeeDetailsDisplayed.InvokeAsync(Employee.Id);
            NavigationManager.NavigateTo($"/employeedetails/{Employee.Id}");
        }

        protected async Task BtnEditEmployee_ClickAsync()
        {
            await OnEmployeeEditDisplayed.InvokeAsync(Employee.Id);
            NavigationManager.NavigateTo($"/editemployee/{Employee.Id}");
        }
    }

    public class ConfirmEmployeeDeleteEventArgs
    {
        public int EmployeeId { get; }
        public bool IsDeleteConfirmed { get; }

        public ConfirmEmployeeDeleteEventArgs(in int employeeId, in bool isDeleteConfirmed)
        {
            EmployeeId = employeeId;
            IsDeleteConfirmed = isDeleteConfirmed;
        }
    }
}
