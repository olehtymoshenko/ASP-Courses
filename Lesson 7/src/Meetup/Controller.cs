namespace Meets.WebApi.Meetup;

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/meetups")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class MeetupController : ControllerBase
{
    private static readonly ICollection<Meetup> Meetups =
        new List<Meetup>();
    
    /// <summary>Create a new meetup.</summary>
    /// <param name="createDto">Meetup creation information.</param>
    /// <response code="200">Newly created meetup.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReadMeetupDto), StatusCodes.Status200OK)]
    public IActionResult CreateMeetup([FromBody] CreateMeetupDto createDto)
    {
        var newMeetup = new Meetup
        {
            Id = Guid.NewGuid(),
            Topic = createDto.Topic,
            Place = createDto.Place,
            Duration = createDto.Duration
        };
        Meetups.Add(newMeetup);

        var readDto = new ReadMeetupDto
        {
            Id = newMeetup.Id,
            Topic = newMeetup.Topic,
            Place = newMeetup.Place,
            Duration = newMeetup.Duration
        };
        return Ok(readDto);
    }

    /// <summary>Retrieve all meetups.</summary>
    /// <response code="200">All meetups.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReadMeetupDto[]), StatusCodes.Status200OK)]
    public IActionResult GetAllMeetups()
    {
        var readDtos = Meetups.Select(meetup => new ReadMeetupDto
        {
            Id = meetup.Id,
            Topic = meetup.Topic,
            Place = meetup.Place,
            Duration = meetup.Duration
        });
        
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
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] UpdateMeetupDto updateDto)
    {
        var oldMeetup = Meetups.SingleOrDefault(meetup => meetup.Id == id);
        if (oldMeetup is null)
        {
            return NotFound();
        }

        oldMeetup.Topic = updateDto.Topic;
        oldMeetup.Place = updateDto.Place;
        oldMeetup.Duration = updateDto.Duration;
        return NoContent();
    }

    /// <summary>Delete meetup with matching id.</summary>
    /// <param name="id" example="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx">Meetup id.</param>
    /// <response code="200">Deleted meetup.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ReadMeetupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteMeetup([FromRoute] Guid id)
    {
        var meetupToDelete = Meetups.SingleOrDefault(meetup => meetup.Id == id);
        if (meetupToDelete is null)
        {
            return NotFound();
        }
        Meetups.Remove(meetupToDelete);

        var readDto = new ReadMeetupDto
        {
            Id = meetupToDelete.Id,
            Topic = meetupToDelete.Topic,
            Place = meetupToDelete.Place,
            Duration = meetupToDelete.Duration
        };
        return Ok(readDto);
    }
}
