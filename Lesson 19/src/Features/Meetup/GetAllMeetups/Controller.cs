namespace Meets.WebApi.Features.Meetup.GetAllMeetups;

using System.Net.Mime;
using AutoMapper;
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
    
    /// <summary>Retrieve all meetups.</summary>
    /// <response code="200">All meetups.</response>
    [HttpGet("/meetups")]
    [ProducesResponseType(typeof(MeetupDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMeetups()
    {
        var meetups = await _context.Meetups
            .Include(meetup => meetup.SignedUpUsers)
            .ToListAsync();
        
        var readDtos = _mapper.Map<ICollection<MeetupDto>>(meetups);
        return Ok(readDtos);
    }
}