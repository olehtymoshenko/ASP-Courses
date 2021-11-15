namespace Meets.WebApi.Meetup;

using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/meetups")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class MeetupController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public MeetupController(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>Create a new meetup.</summary>
    /// <param name="createDto">Meetup creation information.</param>
    /// <response code="200">Newly created meetup.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReadMeetupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateMeetup([FromBody] CreateMeetupDto createDto)
    {
        var newMeetup = _mapper.Map<MeetupEntity>(createDto);
        
        _context.Meetups.Add(newMeetup);
        await _context.SaveChangesAsync();

        var readDto = _mapper.Map<ReadMeetupDto>(newMeetup);
        return Ok(readDto);
    }

    /// <summary>Retrieve all meetups.</summary>
    /// <response code="200">All meetups.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReadMeetupDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMeetups()
    {
        var meetups = await _context.Meetups.ToListAsync();
        var readDtos = _mapper.Map<ICollection<ReadMeetupDto>>(meetups);
        return Ok(readDtos);
    }

    /// <summary>Update meetup with matching id.</summary>
    /// <param name="id" example="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx">Meetup id.</param>
    /// <param name="updateDto">Meetup update information.</param>
    /// <response code="204">Meetup was updated successfully.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMeetup([FromRoute] Guid id, [FromBody] UpdateMeetupDto updateDto)
    {
        var oldMeetup = await _context.Meetups.SingleOrDefaultAsync(meetup => meetup.Id == id);
        if (oldMeetup is null)
        {
            return NotFound();
        }

        _mapper.Map(updateDto, oldMeetup);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    /// <summary>Delete meetup with matching id.</summary>
    /// <param name="id" example="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx">Meetup id.</param>
    /// <response code="200">Deleted meetup.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ReadMeetupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMeetup([FromRoute] Guid id)
    {
        var meetupToDelete = await _context.Meetups.SingleOrDefaultAsync(meetup => meetup.Id == id);
        if (meetupToDelete is null)
        {
            return NotFound();
        }
        
        _context.Meetups.Remove(meetupToDelete);
        await _context.SaveChangesAsync();

        var readDto = _mapper.Map<ReadMeetupDto>(meetupToDelete);
        return Ok(readDto);
    }
}
