using AutoMapper;

namespace BlazorDemo.Models
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
