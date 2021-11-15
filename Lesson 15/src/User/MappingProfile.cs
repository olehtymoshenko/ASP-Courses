namespace Meets.WebApi.User;

using AutoMapper;
using Meets.WebApi.Helpers;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, ReadUserDto>();
        CreateMap<RegisterUserDto, UserEntity>();
        CreateMap<JwtTokenHelper.TokenPair, TokenPairDto>();
    }
}
