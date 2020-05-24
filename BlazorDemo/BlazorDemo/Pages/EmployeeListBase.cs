using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages
{
    public class EmployeeListBase : ComponentBase
    {
        protected string DeletedEmployeeMessage { get; set; }

        [Inject] 
        public IEmployeeService EmployeeService { get; set; }

        public IEnumerable<Employee> Employees { get; set; }

        public bool ShowFooter { get; set; } = true;

        public int SelectedEmployeesCount { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReloadEmployeeList();
        }

        protected void Employee_Selected(bool isEmployeeSelected)
        {
            SelectedEmployeesCount = isEmployeeSelected ? SelectedEmployeesCount + 1 : SelectedEmployeesCount - 1;
        }

        protected async Task Employee_DeletedAsync(int deletedEmployeeId)
        {
            var deletedEmployee = Employees.Single(e => e.Id == deletedEmployeeId);
            DeletedEmployeeMessage = $"Employee (id = {deletedEmployeeId}): \"{deletedEmployee.FirstName} {deletedEmployee.LastName}\" has been deleted";
            await ReloadEmployeeList();
            
        }

        private async Task ReloadEmployeeList()
        {
            try
            {
                ErrorMessage = null;
                Employees = await EmployeeService.GetEmployeesAsync();
            }
            catch (Exception)
            {
                ErrorMessage = $"There was a problem while retrieving data through {nameof(EmployeeService)}";
                Employees = new List<Employee>();
            }
        }
    }
}
