namespace Meets.WebApi.Features.User.RegisterNewUser;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserDto, UserEntity>();
        CreateMap<UserEntity, RegisteredUserDto>();
    }
}