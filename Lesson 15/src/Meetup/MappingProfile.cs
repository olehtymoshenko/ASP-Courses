namespace Meets.WebApi.Meetup;

using AutoMapper;

internal class MeetupMappingProfile : Profile
{
    public MeetupMappingProfile()
    {
        CreateMap<MeetupEntity, ReadMeetupDto>();
        CreateMap<CreateMeetupDto, MeetupEntity>();
        CreateMap<UpdateMeetupDto, MeetupEntity>();
    }
}
