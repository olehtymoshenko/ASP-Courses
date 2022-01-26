namespace Meets.WebApi.Features.Meetup.UpdateMeetup;

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
    
    /// <summary>Update meetup with matching id.</summary>
    /// <param name="id" example="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx">Meetup id.</param>
    /// <param name="updateDto">Meetup update information.</param>
    /// <response code="204">Meetup was updated successfully.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpPut("/meetups/{id:guid}")]
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
}