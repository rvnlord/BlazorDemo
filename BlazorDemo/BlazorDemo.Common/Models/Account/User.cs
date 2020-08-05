using System;
using Microsoft.AspNetCore.Identity;

namespace BlazorDemo.Common.Models.Account
{
    public class User : IdentityUser<Guid>
    {
        public string EmailActivationToken { get; set; }
    } 
}
