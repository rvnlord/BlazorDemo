using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemo.Common.Converters;
using BlazorDemo.Models;
using BlazorDemo.Services;
using CommonLibrary.Components;
using CommonLibrary.Extensions;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BlazorDemo.Pages
{
    public class EditEmployeeBase : ComponentBase, IDisposable
    {
        protected List<string> Routes { get; set; } = new List<string>();

        public string PageHeaderText { get; set; }
        
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        public IDepartmentService DepartmentService { get; set; }

        [Inject] 
        public IMapper Mapper { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IParametersService ParametersService { get; set; }

        public EditEmployeeVM EditEmployeeVM { get; set; } = new EditEmployeeVM();

        public List<Department> AvailableDepartments { get; set; } = new List<Department>();

        public ConfirmationDialogBase DeleteConfirmation { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            var id = GetIdFromLocation();
            if (id == null)
                throw new NullReferenceException($"{nameof(id)} is 'null' which means that path is not the one for 'Edit' or 'Create Employee', it shouldn't have happened");

            await ReinitializeComponent((int) id);
            await JsRuntime.InvokeVoidAsync("blazor_EditEmployee_LocationChangedToCurrent", id);
        }

        private async void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            var id = GetIdFromLocation();

            if (id != null)
                await ReinitializeComponent((int) id);
            else // if id is 'null' then we got path to other component
                NavigationManager.LocationChanged -= NavigationManager_LocationChanged;

            await JsRuntime.InvokeVoidAsync("blazor_EditEmployee_LocationChangedToCurrent", id);
        }

        private async Task ReinitializeComponent(int id)
        {
            PageHeaderText = id != 0 ? "Edit Employee" : "Create Employee";
            EditEmployeeVM = (id != 0
                ? await EmployeeService.GetEmployeeByIdAsync(id)
                : new Employee
                {
                    DepartmentId = 1,
                    DateOfBirth = DateTime.Now, 
                    PhotoPath = "images/nophoto.jpg"
                }).ToEditEmployeeVM(Mapper);
            AvailableDepartments = (await DepartmentService.GetDepartmentsAsync()).ToList();
            StateHasChanged();
            await JsRuntime.InvokeVoidAsync("blazor_EditEmployee_Reinitialized");
        }

        private int? GetIdFromLocation()
        {
            var location = NavigationManager.Uri.EndsWith("/") ? NavigationManager.Uri.SkipLast(1) : NavigationManager.Uri;
            var baseUrl = NavigationManager.BaseUri.SkipLast(1);
            var routeSplit = (baseUrl + Routes.Single(r => r.Contains("{id}"))).Split('{', '}');
            int? id = 0;

            if (location.StartsWith(routeSplit[0]) && location.EndsWith(routeSplit[2]))
                id = int.Parse(location.Skip(routeSplit[0].Length).TakeWhile(char.IsDigit));

            if (!Routes.Select(r => baseUrl + r.Replace("{id}", id.ToString())).Select(r => r.EndsWith("/") ? r.SkipLast(1) : r).Any(r => r.EqualsInvariantIgnoreCase(location)))
                id = null;

            Id = id?.ToString();
            return id;
        }

        protected async Task EditForm_ValidSubmitAsync()
        {
            try
            {
                var updatedEmployee = EditEmployeeVM.Id != 0 
                    ? await EmployeeService.UpdateEmployeeAsync(EditEmployeeVM.ToEmployee(Mapper)) 
                    : await EmployeeService.CreateEmployeeAsync(EditEmployeeVM.ToEmployee(Mapper));
                if (updatedEmployee != null)
                    NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                ParametersService.Parameters[nameof(PromptBase.Message)] = ex.Message;
                NavigationManager.NavigateTo("/error");
            }
        }

        protected void BtnDelete_Click()
        {
            DeleteConfirmation.Show();
        }

        protected async Task BtnConfirmDelete_ClickAsync(bool isDeleteConfirmed)
        {
            if (isDeleteConfirmed)
            {
                await EmployeeService.DeleteEmployeeAsync(EditEmployeeVM.Id);
                NavigationManager.NavigateTo("/");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) 
                return;

            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }
    
        ~EditEmployeeBase()
        {
            Dispose(false);
        }
    }
}
