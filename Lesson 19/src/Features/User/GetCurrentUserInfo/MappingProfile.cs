namespace Meets.WebApi.Features.User.GetCurrentUserInfo;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<UserEntity, CurrentUserInfoDto>();
}