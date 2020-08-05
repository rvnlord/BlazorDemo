using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using Microsoft.Collections.Extensions;

namespace BlazorDemo.Common.Services.Frontend.Admin.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<List<FindUserVM>>> GetAllUsersAsync();
        Task<ApiResponse<AdminEditUserVM>> DeleteUserAsync(AdminEditUserVM user);
        Task<ApiResponse<AdminEditUserVM>> EditUserAsync(AdminEditUserVM user);
        Task<ApiResponse<List<FindRoleVM>>> GetRolesAsync();
        Task<ApiResponse<List<FindClaimVM>>> GetClaimsAsync();
        Task<ApiResponse<AdminEditUserVM>> AddUserAsync(AdminEditUserVM user);
        ApiResponse<FindRoleVM> FindRoleByName(string roleName);
        Task<ApiResponse<AdminEditRoleVM>> DeleteRoleAsync(AdminEditRoleVM role);
        Task<ApiResponse<AdminEditRoleVM>> AddRoleAsync(AdminEditRoleVM role);
        Task<ApiResponse<AdminEditRoleVM>> EditRoleAsync(AdminEditRoleVM role);
        Task<ApiResponse<FindRoleVM>> FindRoleByIdAsync(Guid id);
        ApiResponse<FindClaimVM> FindClaimByName(string claimName);
        Task<ApiResponse<AdminEditClaimVM>> DeleteClaimAsync(AdminEditClaimVM claim);
        Task<ApiResponse<AdminEditClaimVM>> AddClaimAsync(AdminEditClaimVM claim);
        Task<ApiResponse<FindUserVM>> FindUserByIdAsync(Guid id);
        Task<ApiResponse<FindClaimVM>> FindClaimByNameAsync(string name);
        Task<ApiResponse<AdminEditClaimVM>> EditClaimAsync(AdminEditClaimVM claim);
    }
}
