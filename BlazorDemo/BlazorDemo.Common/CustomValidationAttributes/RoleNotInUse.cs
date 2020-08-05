using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class RoleNotInUse : NotInUse
    {
        public RoleNotInUse() : base(typeof(IAdminService), nameof(IAdminService.FindRoleByName)) { }
    }
}
