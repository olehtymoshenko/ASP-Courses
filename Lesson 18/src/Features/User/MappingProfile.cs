namespace Meets.WebApi.Features.User;

using AutoMapper;
using Meets.WebApi.Features.User.DataTransferObjects;
using Meets.WebApi.Features.User.Entities;
using Meets.WebApi.Features.User.Helpers;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, ReadUserDto>();
        CreateMap<RegisterUserDto, UserEntity>();
        CreateMap<TokenPair, TokenPairDto>();
    }
}
