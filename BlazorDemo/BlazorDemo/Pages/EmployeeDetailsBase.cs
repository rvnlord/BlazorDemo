using System.Threading.Tasks;
using BlazorDemo.Common.Utils.UtilClasses;
using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorDemo.Pages
{
    public class EmployeeDetailsBase : ComponentBase
    {
        protected Point Coordinates { get; set; }
        protected string ToggleFooterButtonText { get; set; } = "Hide Footer";
        protected string FooterCssClass { get; set; } = null;

        public Employee Employee { get; set; } = new Employee();

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
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
    }
}
