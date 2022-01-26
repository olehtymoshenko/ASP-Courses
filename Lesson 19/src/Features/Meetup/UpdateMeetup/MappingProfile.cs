namespace Meets.WebApi.Features.Meetup.UpdateMeetup;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<UpdateMeetupDto, MeetupEntity>();
}