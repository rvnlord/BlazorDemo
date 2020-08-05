using BlazorDemo.Common.Services.Frontend.Account.Interfaces;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class UserNameNotInUse : NotInUse
    {
        public UserNameNotInUse() : base(typeof(IAccountService), nameof(IAccountService.FindUserByName)) { }
    }
}
