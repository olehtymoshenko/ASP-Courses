namespace Meets.WebApi.Features.User.RefreshTokenPair;

using AutoMapper;
using Meets.WebApi.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<TokenPair, TokenPairDto>();
}