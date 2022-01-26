namespace Meets.WebApi.Features.Meetup.DeleteMeetup;

using AutoMapper;
using Meets.WebApi.Entities;

public class MappingProfile : Profile
{
    public MappingProfile() =>
        CreateMap<MeetupEntity, DeletedMeetupDto>();
}