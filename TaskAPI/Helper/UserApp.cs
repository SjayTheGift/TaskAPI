using AutoMapper;
using TaskAPI.Dto;
using TaskAPI.Models;

namespace TaskAPI.Helper
{
    public class UserApp : Profile
    {
        public UserApp()
        {
            CreateMap<User, UserDto>();
        }
    }
}
