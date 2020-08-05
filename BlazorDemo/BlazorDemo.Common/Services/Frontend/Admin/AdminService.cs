using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using Blazored.LocalStorage;
using Microsoft.Collections.Extensions;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;

namespace BlazorDemo.Common.Services.Frontend.Admin
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccountService _accountService;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;

        public AdminService(HttpClient httpClient, IAccountService accountService, IJSRuntime jsRuntime, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _accountService = accountService;
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
        }

        public static IAdminService Create(HttpClient httpClient)
        {
            return new AdminService(httpClient, null, null, null);
        }

        public async Task<ApiResponse<List<FindUserVM>>> GetAllUsersAsync()
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                var usersResp = await _httpClient.PostJTokenAsync<ApiResponse<List<FindUserVM>>>("api/admin/users", authUser);
                usersResp.Result ??= new List<FindUserVM>();
                return usersResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FindUserVM>>(StatusCodeType.Status500InternalServerError, "API threw an exception while retrieving Users", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditUserVM>> DeleteUserAsync(AdminEditUserVM userToDelete)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditUserVM>>("api/admin/deleteuser", new
                {
                    AuthenticatedUser = authUser, 
                    UserToDelete = userToDelete
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while deleting User", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditUserVM>> EditUserAsync(AdminEditUserVM userToEdit)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                var editUserResp = await _httpClient.PostJTokenAsync<ApiResponse<AdminEditUserVM>>("api/admin/edituser", new
                {
                    AuthenticatedUser = authUser,
                    UserToEdit = userToEdit
                });

                if (editUserResp.IsError || editUserResp.Result?.Ticket == null) 
                    return editUserResp;

                if (authUser?.RememberMe == true)
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", editUserResp.Result.Ticket, new { expires = 365 * 24 * 60 * 60 });
                    await _localStorage.SetItemAsync("Ticket",  editUserResp.Result.Ticket);
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("Cookies.set", "Ticket", editUserResp.Result.Ticket);
                    await _localStorage.RemoveItemAsync("Ticket");
                }

                return editUserResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while editing User", null, null, ex);
            }
        }

        public async Task<ApiResponse<List<FindRoleVM>>> GetRolesAsync()
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                var rolesResp = await _httpClient.PostJTokenAsync<ApiResponse<List<FindRoleVM>>>("api/admin/getroles", authUser);
                rolesResp.Result ??= new List<FindRoleVM>();
                return rolesResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FindRoleVM>>(StatusCodeType.Status500InternalServerError, "API threw an exception while retrieving Roles", null, null, ex);
            }
        }

        public async Task<ApiResponse<List<FindClaimVM>>> GetClaimsAsync()
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                var claimsResp = await _httpClient.PostJTokenAsync<ApiResponse<List<FindClaimVM>>>("api/admin/getclaims", authUser);
                claimsResp.Result ??= new List<FindClaimVM>(); // by default json serializer will serialize an empty array/list as null, so if there are no claims, we will get null from the api 
                return claimsResp;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FindClaimVM>>(StatusCodeType.Status500InternalServerError, "API threw an exception while retrieving Claims", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditUserVM>> AddUserAsync(AdminEditUserVM userToAdd)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditUserVM>>("api/admin/adduser", new
                {
                    AuthenticatedUser = authUser,
                    UserToAdd = userToAdd
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditUserVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while adding new User", null, null, ex);
            }
        }

        public ApiResponse<FindRoleVM> FindRoleByName(string roleName)
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/admin/findrolebyname", new JObject { ["roleName"] = roleName }.JsonSerialize()).JsonDeserialize().To<ApiResponse<FindRoleVM>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindRoleVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find Role by Name", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditRoleVM>> DeleteRoleAsync(AdminEditRoleVM roleToDelete)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditRoleVM>>("api/admin/deleterole", new
                {
                    AuthenticatedUser = authUser, 
                    RoleToDelete = roleToDelete
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditRoleVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while deleting Role", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditRoleVM>> AddRoleAsync(AdminEditRoleVM roleToAdd)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditRoleVM>>("api/admin/addrole", new
                {
                    AuthenticatedUser = authUser,
                    RoleToAdd = roleToAdd
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditRoleVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while adding new Role", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditRoleVM>> EditRoleAsync(AdminEditRoleVM roleToEdit)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditRoleVM>>("api/admin/editrole", new
                {
                    AuthenticatedUser = authUser,
                    RoleToEdit = roleToEdit
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditRoleVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while editing Role", null, null, ex);
            }
        }

        public async Task<ApiResponse<FindRoleVM>> FindRoleByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<FindRoleVM>>("api/admin/findrolebyid", id);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindRoleVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find Role by Id", null, null, ex);
            }
        }

        public ApiResponse<FindClaimVM> FindClaimByName(string claimName)
        {
            try
            {
                var wc = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } };
                return wc.UploadString($"{_httpClient.BaseAddress}api/admin/findclaimbyname", new JObject { ["claimName"] = claimName }.JsonSerialize()).JsonDeserialize().To<ApiResponse<FindClaimVM>>();
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindClaimVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find Claim by Name", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditClaimVM>> DeleteClaimAsync(AdminEditClaimVM claimToDelete)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditClaimVM>>("api/admin/deleteclaim", new
                {
                    AuthenticatedUser = authUser, 
                    ClaimToDelete = claimToDelete
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditClaimVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while deleting Claim", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditClaimVM>> AddClaimAsync(AdminEditClaimVM claimToAdd)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditClaimVM>>("api/admin/addclaim", new
                {
                    AuthenticatedUser = authUser,
                    ClaimToAdd = claimToAdd
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditClaimVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while adding new Claim", null, null, ex);
            }
        }

        public async Task<ApiResponse<FindUserVM>> FindUserByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<FindUserVM>>("api/admin/finduserbyid", id);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindUserVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find User by Id", null, null, ex);
            }
        }

        public async Task<ApiResponse<FindClaimVM>> FindClaimByNameAsync(string name)
        {
            try
            {
                return await _httpClient.PostJTokenAsync<ApiResponse<FindClaimVM>>("api/admin/findclaimbynameasync", name);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FindClaimVM>(StatusCodeType.Status500InternalServerError, "API threw an Exception while trying to Find Claim by Name", null, null, ex);
            }
        }

        public async Task<ApiResponse<AdminEditClaimVM>> EditClaimAsync(AdminEditClaimVM claimToedit)
        {
            try
            {
                var authUser = (await _accountService.GetAuthenticatedUserAsync())?.Result;
                return await _httpClient.PostJTokenAsync<ApiResponse<AdminEditClaimVM>>("api/admin/editclaim", new
                {
                    AuthenticatedUser = authUser,
                    ClaimToEdit = claimToedit
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AdminEditClaimVM>(StatusCodeType.Status500InternalServerError, "API threw an exception while editing Claim", null, null, ex);
            }
        }
    }
}
