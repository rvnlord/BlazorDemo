using System;
using AutoMapper;
using BlazorDemo.Common.Models.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BlazorDemo.Common.Models.Admin.MappingProfiles
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<IdentityRole<Guid>, FindRoleVM>();
            CreateMap<FindRoleVM, AdminEditRoleVM>();
            CreateMap<AdminEditRoleVM, IdentityRole<Guid>>();
        }
    }
}
