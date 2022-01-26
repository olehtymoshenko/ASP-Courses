namespace Meets.WebApi.Features.Meetup.GetAllMeetups;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<MeetupEntity, MeetupDto>()
            .ForMember(readDto => readDto.SignedUp, config => config.MapFrom(meetup => meetup.SignedUpUsers!.Count));
}