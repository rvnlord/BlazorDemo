using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;

namespace BlazorDemo.Common.Services.Backend.Admin.Interfaces
{
    public interface IAdminManager
    {
        Task<ApiResponse<List<FindUserVM>>> GetAllUsersAsync(AuthenticateUserVM authUser);
        Task<ApiResponse<AdminEditUserVM>> DeleteUserAsync(AuthenticateUserVM authUser, AdminEditUserVM userToDelete);
        Task<ApiResponse<AdminEditUserVM>> EditUserAsync(AuthenticateUserVM authUser, AdminEditUserVM userToEdit);
        Task<ApiResponse<List<FindRoleVM>>> GetRolesAsync(AuthenticateUserVM authUser);
        Task<ApiResponse<List<FindClaimVM>>> GetClaimsAsync(AuthenticateUserVM authUser);
        Task<ApiResponse<AdminEditUserVM>> AddUserAsync(AuthenticateUserVM authUser, AdminEditUserVM userToAdd);
        ApiResponse<FindRoleVM> FindRoleByName(string roleName);
        Task<ApiResponse<AdminEditRoleVM>> DeleteRoleAsync(AuthenticateUserVM authUser, AdminEditRoleVM roleToDelete);
        Task<ApiResponse<AdminEditRoleVM>> AddRoleAsync(AuthenticateUserVM authUser, AdminEditRoleVM roleToAdd);
        Task<ApiResponse<AdminEditRoleVM>> EditRoleAsync(AuthenticateUserVM authUser, AdminEditRoleVM roleToEdit);
        Task<ApiResponse<FindRoleVM>> FindRoleByIdAsync(Guid id);
        ApiResponse<FindClaimVM> FindClaimByName(string claimName);
        Task<ApiResponse<AdminEditClaimVM>> DeleteClaimAsync(AuthenticateUserVM authUser, AdminEditClaimVM claimToDelete);
        Task<ApiResponse<bool>> HasClaimAsync(User user, string claimName);
        Task<ApiResponse<AdminEditClaimVM>> AddClaimAsync(AuthenticateUserVM authUser, AdminEditClaimVM claimToAdd);
        Task<ApiResponse<FindUserVM>> FindUserByIdAsync(Guid id);
        Task<ApiResponse<FindClaimVM>> FindClaimByNameAsync(string claimName);
        Task<ApiResponse<AdminEditClaimVM>> EditClaimAsync(AuthenticateUserVM authUser, AdminEditClaimVM claimToEdit);
    }
}
