namespace Meets.WebApi.Features.Meetup.RegisterNewMeetup;

using System.Net.Mime;
using AutoMapper;
using Meets.WebApi.Entities;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Mvc;

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

    /// <summary>Create a new meetup.</summary>
    /// <param name="createDto">Meetup creation information.</param>
    /// <response code="200">Newly created meetup.</response>
    [HttpPost("/meeetups")]
    [ProducesResponseType(typeof(RegisteredMeetupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateMeetup([FromBody] RegisterMeetupDto createDto)
    {
        var newMeetup = _mapper.Map<MeetupEntity>(createDto);
        
        _context.Meetups.Add(newMeetup);
        await _context.SaveChangesAsync();

        var readDto = _mapper.Map<RegisteredMeetupDto>(newMeetup);
        return Ok(readDto);
    }
}