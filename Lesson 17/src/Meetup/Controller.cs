namespace Meets.WebApi.Meetup;

using System.Net.Mime;
using AutoMapper;
using Meets.WebApi;
using Microsoft.AspNetCore.Authorization;
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
        var meetups = await _context.Meetups
            .Include(meetup => meetup.SignedUpUsers)
            .ToListAsync();
        
        var readDtos = _mapper.Map<ICollection<ReadMeetupDto>>(meetups);
        return Ok(readDtos);
    }

    /// <summary>Sign up for meetup with specified id.</summary>
    /// <param name="id">Meetup id.</param>
    /// <response code="200">Signed up successfully.</response>
    /// <response code="404">Meetup with the specified id does not exist.</response>
    /// <response code="409">You've been already signed up for this meetup.</response>
    [HttpPost("{id:guid}/sign-up")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUpForMeetup([FromRoute] Guid id)
    {
        var subClaim = User.Claims.Single(claim => claim.Type == "sub");
        var currentUserId = Guid.Parse(subClaim.Value);

        var meetup = await _context.Meetups
            .Include(meetup => meetup.SignedUpUsers)
            .SingleOrDefaultAsync(meetup => meetup.Id == id);
        if (meetup is null)
        {
            return NotFound();
        }

        var isAlreadySigned = meetup.SignedUpUsers.Any(user => user.Id == currentUserId);
        if (isAlreadySigned)
        {
            return Conflict("You've already signed up for this meetup.");
        }

        var currentUser = await _context.Users.SingleAsync(user => user.Id == currentUserId);
        meetup.SignedUpUsers.Add(currentUser);
        await _context.SaveChangesAsync();

        return Ok();
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
        var meetupToDelete = await _context.Meetups
            .Include(meetup => meetup.SignedUpUsers)
            .SingleOrDefaultAsync(meetup => meetup.Id == id);
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
