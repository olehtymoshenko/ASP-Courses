namespace Meets.WebApi.Features.Meetup;

using AutoMapper;
using Meets.WebApi.Features.Meetup.DataTransferObjects;
using Meets.WebApi.Features.Meetup.Entities;

internal class MeetupMappingProfile : Profile
{
    public MeetupMappingProfile()
    {
        CreateMap<MeetupEntity, ReadMeetupDto>()
            .ForMember(readDto => readDto.SignedUp, config => config.MapFrom(meetup => meetup.SignedUpUsers!.Count));
        CreateMap<CreateMeetupDto, MeetupEntity>();
        CreateMap<UpdateMeetupDto, MeetupEntity>();
    }
}
