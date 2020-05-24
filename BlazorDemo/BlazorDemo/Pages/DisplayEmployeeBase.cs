﻿using System.Threading.Tasks;
using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages
{
    public class DisplayEmployeeBase : ComponentBase
    {
        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        protected string SelectEmployeeCheckBoxId { get; set; }

        protected bool IsEmployeeSelected { get; set; }

        [Parameter]
        public EventCallback<bool> OnEmployeeSelection { get; set; }
        
        [Parameter]
        public EventCallback<int> OnEmployeeDeleted { get; set; }

        [Parameter]
        public Employee Employee { get; set; }

        [Parameter]
        public bool ShowFooter { get; set; }

        protected override Task OnInitializedAsync()
        {
            SelectEmployeeCheckBoxId = $"cbSelect{Employee.FirstName}{@Employee.LastName}";
            return base.OnInitializedAsync();
        }

        protected async Task CbSelectEmployee_ChangedAsync(ChangeEventArgs e)
        {
            IsEmployeeSelected = (bool) e.Value;
            await OnEmployeeSelection.InvokeAsync(IsEmployeeSelected);
        }

        protected async Task BtnDelete_ClickAsync()
        {
            await EmployeeService.DeleteEmployeeAsync(Employee.Id);
            await OnEmployeeDeleted.InvokeAsync(Employee.Id);
        }
    }
}
