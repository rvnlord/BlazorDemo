using BlazorDemo.Common.Services.Frontend.Account.Interfaces;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class EmailNotInUse : NotInUse 
    {
        public EmailNotInUse() : base(typeof(IAccountService), nameof(IAccountService.FindUserByEmail)) { }
    }
}
