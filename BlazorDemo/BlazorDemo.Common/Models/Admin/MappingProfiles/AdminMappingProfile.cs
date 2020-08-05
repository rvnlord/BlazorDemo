using AutoMapper;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.ViewModels;
using BlazorDemo.Common.Models.Admin.ViewModels;

namespace BlazorDemo.Common.Models.Admin.MappingProfiles
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<User, AdminEditUserVM>();
            CreateMap<BlazorDemo.Common.Models.Account.ViewModels.FindUserVM, BlazorDemo.Common.Models.Admin.ViewModels.AdminEditUserVM>();
        }
    }
}
