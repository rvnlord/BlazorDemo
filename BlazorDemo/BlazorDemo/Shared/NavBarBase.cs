using System;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.Account.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Shared
{
    public class NavBarBase : ComponentBase
    {
        [CascadingParameter]
        public string CurrentUrl { get; set; }

        [CascadingParameter]
        public AuthenticateUserVM AuthenticatedUser { get; set; }

        [CascadingParameter]
        public Func<Task> BtnLogout_ClickAsync { get; set; }
    }
}
