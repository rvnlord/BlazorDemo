using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;

namespace BlazorDemo.Common.CustomValidationAttributes
{
    public class ClaimNotInUse : NotInUse 
    {
        public ClaimNotInUse() : base(typeof(IAdminService), nameof(IAdminService.FindClaimByName)) { }
    }
}
