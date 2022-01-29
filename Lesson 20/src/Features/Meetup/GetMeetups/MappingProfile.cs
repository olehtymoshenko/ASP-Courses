namespace Meets.WebApi.Features.Meetup.GetMeetups;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<MeetupEntity, ResponseDto.MeetupDto>()
            .ForMember(readDto => readDto.SignedUp, config => config.MapFrom(meetup => meetup.SignedUpUsers!.Count));
}