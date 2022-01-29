namespace Meets.WebApi.Features.Meetup.DeleteMeetup;

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

    /// <summary>Delete meetup with matching id.</summary>
    /// <param name="id" example="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx">Meetup id.</param>
    /// <response code="200">Deleted meetup.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpDelete("/meetups/{id:guid}")]
    [ProducesResponseType(typeof(DeletedMeetupDto), StatusCodes.Status200OK)]
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

        var readDto = _mapper.Map<DeletedMeetupDto>(meetupToDelete);
        return Ok(readDto);
    }
}