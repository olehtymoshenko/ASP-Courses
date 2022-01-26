namespace Meets.WebApi.Features.Meetup.RegisterNewMeetup;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterMeetupDto, MeetupEntity>();
        CreateMap<MeetupEntity, RegisteredMeetupDto>();
    }
}