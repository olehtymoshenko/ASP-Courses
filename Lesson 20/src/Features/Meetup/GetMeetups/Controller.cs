namespace Meets.WebApi.Features.Meetup.GetMeetups;

using System.Net.Mime;
using AutoMapper;
using Meets.WebApi.Entities;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Tags("Meetups")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class Controller : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public Controller(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>Retrieve meetups.</summary>
    /// <param name="request">Query parameters.</param>
    /// <response code="200">Retrieved meetups.</response>
    /// <response code="400">Invalid query parameters.</response>
    [HttpGet("/meetups")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMeetups([FromQuery] RequestDto request)
    {
        var allMeetups = _context.Meetups.AsNoTracking().Include(meetup => meetup.SignedUpUsers);
        var filteredMeetups = FilterMeetups(allMeetups, request.Filters);

        var meetupsCount = await filteredMeetups.CountAsync();
        var pagesCount = (int) Math.Ceiling((double) meetupsCount / request.Pagination.PageSize);

        var paginatedMeetups = PaginateMeetups(filteredMeetups, request.Pagination);
        var meetupDtos = _mapper.Map<ICollection<ResponseDto.MeetupDto>>(await paginatedMeetups.ToListAsync());

        var response = new ResponseDto
        {
            Pagination = new()
            {
                PagesCount = pagesCount,
                MeetupsCount = meetupsCount,
            },
            Meetups = meetupDtos
        };
        return Ok(response);
    }

    private static IQueryable<MeetupEntity> FilterMeetups(
        IQueryable<MeetupEntity> meetups,
        RequestDto.FiltersDto filters) =>
        meetups
            .Where(meetup => filters.MinDuration == null || meetup.Duration >= filters.MinDuration)
            .Where(meetup => filters.MaxDuration == null || meetup.Duration <= filters.MaxDuration)
            .Where(meetup => filters.MinSignedUp == null || meetup.SignedUpUsers!.Count >= filters.MinSignedUp)
            .Where(meetup => filters.MaxSignedUp == null || meetup.SignedUpUsers!.Count <= filters.MaxSignedUp)
            .Where(meetup => string.IsNullOrWhiteSpace(filters.Search) ||
                             EF.Functions.Like(meetup.Topic, $"%{filters.Search}%") ||
                             EF.Functions.Like(meetup.Place, $"%{filters.Search}%"));

    private static IQueryable<MeetupEntity> PaginateMeetups(
        IQueryable<MeetupEntity> meetups,
        RequestDto.PaginationDto pagination)
    {
        var orderedMeetups = pagination.Order switch
        {
            RequestDto.PaginationDto.OrderingOption.TopicAlphabetically =>
                meetups.OrderBy(meetup => meetup.Topic),
            RequestDto.PaginationDto.OrderingOption.TopicReverseAlphabetically =>
                meetups.OrderByDescending(meetup => meetup.Topic),
            RequestDto.PaginationDto.OrderingOption.DurationAscending =>
                meetups.OrderBy(meetup => meetup.Duration),
            RequestDto.PaginationDto.OrderingOption.DurationDescending =>
                meetups.OrderByDescending(meetup => meetup.Duration),
            RequestDto.PaginationDto.OrderingOption.SignedUpAscending =>
                meetups.OrderBy(meetup => meetup.SignedUpUsers!.Count),
            RequestDto.PaginationDto.OrderingOption.SignedUpDescending =>
                meetups.OrderByDescending(meetup => meetup.SignedUpUsers!.Count),
            _ => meetups
        };
        
        return orderedMeetups
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }
}