using AutoMapper;
using BlazorDemo.Common.Models.EmployeeManagement.ViewModels;

namespace BlazorDemo.Common.Models.EmployeeManagement.MappingProfiles
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EditEmployeeVM>().ForMember(dest => dest.ConfirmEmail, o => o.MapFrom(src => src.Email));
            CreateMap<EditEmployeeVM, Employee>();
        }
    }
}
