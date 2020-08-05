using AutoMapper;
using BlazorDemo.Common.Models.Account.ViewModels;
using System.Linq;
using BlazorDemo.Common.Models.Admin.ViewModels;

namespace BlazorDemo.Common.Models.Account.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, User>();
            CreateMap<User, RegisterUserVM>();
            CreateMap<User, ConfirmUserVM>();
            CreateMap<User, LoginUserVM>();
            CreateMap<LoginUserVM, LoginUserVM>().ForMember(d => d.ExternalLogins, o => o.Condition(s => s.ExternalLogins?.Any() == true));
            CreateMap<User, AuthenticateUserVM>();
            CreateMap<User, ForgotPasswordUserVM>();
            CreateMap<User, ResetPasswordUserVM>();
            CreateMap<User, ResendEmailConfirmationUserVM>();
            CreateMap<EditUserVM, ResendEmailConfirmationUserVM>();
            CreateMap<User, EditUserVM>();
            CreateMap<EditUserVM, User>();
            CreateMap<AuthenticateUserVM, EditUserVM>();
            CreateMap<EditUserVM, EditUserVM>(); // if this is not set the mapping for same objects will work but will not update the existing dest object only create a new one
            CreateMap<User, AdminEditUserVM>()
                .ForMember(d => d.IsConfirmed, o => o.MapFrom(s => s.EmailConfirmed));
            CreateMap<AdminEditUserVM, User>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.EmailConfirmed, o => o.MapFrom(s => s.IsConfirmed));
            CreateMap<User, FindUserVM>()
                .ForMember(d => d.IsConfirmed, o => o.MapFrom(s => s.EmailConfirmed));
            CreateMap<FindUserVM, AdminEditUserVM>();
            CreateMap<User, FindUserVM>()
                .ForMember(d => d.IsConfirmed, o => o.MapFrom(s => s.EmailConfirmed));
        }
    }
}
