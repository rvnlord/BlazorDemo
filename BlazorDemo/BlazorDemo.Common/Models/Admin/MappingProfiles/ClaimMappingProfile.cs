using System;
using AutoMapper;
using BlazorDemo.Common.Models.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BlazorDemo.Common.Models.Admin.MappingProfiles
{
    public class ClaimMappingProfile : Profile
    {
        public ClaimMappingProfile()
        {
            CreateMap<IdentityUserClaim<Guid>, FindClaimVM>();
            CreateMap<FindClaimVM, AdminEditClaimVM>();
            CreateMap<FindClaimValueVM, AdminEditClaimValueVM>();
            CreateMap<AdminEditClaimVM, IdentityUserClaim<Guid>>();
        }
    }
}
