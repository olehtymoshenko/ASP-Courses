namespace Meets.WebApi.Features.Meetup.SignUpForMeetup;

using System.Net.Mime;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Tags("Meetups")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class Controller : ControllerBase
{
    private readonly DatabaseContext _context;

    public Controller(DatabaseContext context) =>
        _context = context;
    
    /// <summary>Sign up for meetup with specified id.</summary>
    /// <param name="id">Meetup id.</param>
    /// <response code="200">Signed up successfully.</response>
    /// <response code="404">Meetup with the specified id does not exist.</response>
    /// <response code="409">You've been already signed up for this meetup.</response>
    [HttpPost("/meetups/{id:guid}/sign-up")]
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

        var isAlreadySigned = meetup.SignedUpUsers!.Any(user => user.Id == currentUserId);
        if (isAlreadySigned)
        {
            return Conflict("You've already signed up for this meetup.");
        }

        var currentUser = await _context.Users.SingleAsync(user => user.Id == currentUserId);
        meetup.SignedUpUsers!.Add(currentUser);
        await _context.SaveChangesAsync();

        return Ok();
    }
}