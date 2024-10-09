using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
namespace budget_api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<User, UserCreationDto>().ReverseMap();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

        }
    }
}
