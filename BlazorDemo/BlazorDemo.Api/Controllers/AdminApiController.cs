using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Models.Interfaces;
using BlazorDemo.Common.Services.Backend.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Api.Controllers
{
    [Route("api/admin"), ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IApiResponse _defaultInvalidResponse;
        private readonly IAdminManager _adminManager;

        public AdminApiController(IAdminManager adminManager)
        {
            _adminManager = adminManager;
            _defaultInvalidResponse = new ApiResponse(StatusCodeType.Status400BadRequest, "Invalid model", 
                ModelState.Values.SelectMany(v => v.Errors).ToLookup(e => ModelState.Single(s => s.Value.Errors.Contains(e)).Key, e => e.ErrorMessage));
        }

        [HttpPost("users")] // POST: api/admin/users
        public async Task<JToken> GetAllUsersAsync(JToken jAuthenticatedUser) => (ModelState.IsValid ? await _adminManager.GetAllUsersAsync(jAuthenticatedUser.To<AuthenticateUserVM>()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("deleteuser")] // POST: api/admin/deleteuser
        public async Task<JToken> DeleteUserAsync(JToken jAuthenticatedUserAndUserToDelete) => (ModelState.IsValid ? await _adminManager.DeleteUserAsync(jAuthenticatedUserAndUserToDelete["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndUserToDelete["UserToDelete"]?.To<AdminEditUserVM>()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("edituser")] // POST: api/admin/edituser
        public async Task<JToken> EditUserAsync(JToken jAuthenticatedUserAndUserToEdit) => (ModelState.IsValid ? await _adminManager.EditUserAsync(jAuthenticatedUserAndUserToEdit["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndUserToEdit["UserToEdit"]?.To<AdminEditUserVM>()) : _defaultInvalidResponse).ToJToken();
       
        [HttpPost("getroles")] // POST: api/admin/getroles
        public async Task<JToken> GetRolesAsync(JToken jAuthenticatedUser) => (ModelState.IsValid ? await _adminManager.GetRolesAsync(jAuthenticatedUser.To<AuthenticateUserVM>()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("getclaims")] // POST: api/admin/getclaims
        public async Task<JToken> GetClaimsAsync(JToken jAuthenticatedUser) => (ModelState.IsValid ? await _adminManager.GetClaimsAsync(jAuthenticatedUser.To<AuthenticateUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("adduser")] // POST: api/admin/adduser
        public async Task<JToken> AddUserAsync(JToken jAuthenticatedUserAndUserToAdd) => (ModelState.IsValid ? await _adminManager.AddUserAsync(jAuthenticatedUserAndUserToAdd["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndUserToAdd["UserToAdd"]?.To<AdminEditUserVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("findrolebyname")] // POST: api/admin/findrolebyname
        public JToken FindEoleByName(JToken jRoleName) => (ModelState.IsValid ? _adminManager.FindRoleByName(jRoleName["roleName"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("deleterole")] // POST: api/admin/deleterole
        public async Task<JToken> DeleteRoleAsync(JToken jAuthenticatedUserAndRoleToDelete) => (ModelState.IsValid ? await _adminManager.DeleteRoleAsync(jAuthenticatedUserAndRoleToDelete["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndRoleToDelete["RoleToDelete"]?.To<AdminEditRoleVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("addrole")] // POST: api/admin/addrole
        public async Task<JToken> AddRoleAsync(JToken jAuthenticatedUserAndRoleToAdd) => (ModelState.IsValid ? await _adminManager.AddRoleAsync(jAuthenticatedUserAndRoleToAdd["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndRoleToAdd["RoleToAdd"]?.To<AdminEditRoleVM>()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("editrole")] // POST: api/admin/editrole
        public async Task<JToken> EditRoleAsync(JToken jAuthenticatedUserAndRoleToEdit) => (ModelState.IsValid ? await _adminManager.EditRoleAsync(jAuthenticatedUserAndRoleToEdit["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndRoleToEdit["RoleToEdit"]?.To<AdminEditRoleVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("findrolebyid")] // POST: api/admin/findrolebyid
        public async Task<JToken> FindRoleByIdAsync(JToken id) => (ModelState.IsValid ? await _adminManager.FindRoleByIdAsync(id.To<Guid>()) : _defaultInvalidResponse).ToJToken();
        
        [HttpPost("findclaimbyname")] // POST: api/admin/findclaimbyname
        public JToken FindClaimByName(JToken jClaimName) => (ModelState.IsValid ? _adminManager.FindClaimByName(jClaimName["claimName"]?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("deleteclaim")] // POST: api/admin/deleteclaim
        public async Task<JToken> DeleteClaimAsync(JToken jAuthenticatedUserAndClaimToDelete) => (ModelState.IsValid ? await _adminManager.DeleteClaimAsync(jAuthenticatedUserAndClaimToDelete["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndClaimToDelete["ClaimToDelete"]?.To<AdminEditClaimVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("addclaim")] // POST: api/admin/addclaim
        public async Task<JToken> AddClaimAsync(JToken jAuthenticatedUserAndClaimToAdd) => (ModelState.IsValid ? await _adminManager.AddClaimAsync(jAuthenticatedUserAndClaimToAdd["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndClaimToAdd["ClaimToAdd"]?.To<AdminEditClaimVM>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("finduserbyid")] // POST: api/account/finduserbyid
        public async Task<JToken> FindUserByIdAsync(JToken id) => (ModelState.IsValid ? await _adminManager.FindUserByIdAsync(id.To<Guid>()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("findclaimbynameasync")] // POST: api/admin/findclaimbynameasync
        public async Task<JToken> FindClaimByNameAsync(JToken name) => (ModelState.IsValid ? await _adminManager.FindClaimByNameAsync(name?.ToString()) : _defaultInvalidResponse).ToJToken();

        [HttpPost("editclaim")] // POST: api/admin/editclaim
        public async Task<JToken> EditClaimAsync(JToken jAuthenticatedUserAndClaimToEdit) => (ModelState.IsValid ? await _adminManager.EditClaimAsync(jAuthenticatedUserAndClaimToEdit["AuthenticatedUser"]?.To<AuthenticateUserVM>(), jAuthenticatedUserAndClaimToEdit["ClaimToEdit"]?.To<AdminEditClaimVM>()) : _defaultInvalidResponse).ToJToken();
    }
}
