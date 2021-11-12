namespace Meets.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/meetups")]
public class MeetupController : ControllerBase
{
    private static readonly ICollection<Meetup> Meetups =
        new List<Meetup>();

    [HttpPost]
    public IActionResult CreateMeetup([FromBody] Meetup newMeetup)
    {
        newMeetup.Id = Guid.NewGuid();
        Meetups.Add(newMeetup);

        return Ok(newMeetup);
    }

    [HttpGet]
    public IActionResult GetAllMeetups() =>
        Ok(Meetups);

    [HttpPut("{id:guid}")]
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] Meetup updatedMeetup)
    {
        var oldMeetup = Meetups.SingleOrDefault(meetup => meetup.Id == id);

        // meetup with provided id does not exist
        if (oldMeetup is null)
        {
            return NotFound();
        }

        oldMeetup.Topic = updatedMeetup.Topic;
        oldMeetup.Place = updatedMeetup.Place;
        oldMeetup.Duration = updatedMeetup.Duration;

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteMeetup([FromRoute] Guid id)
    {
        var meetupToDelete = Meetups.SingleOrDefault(meetup => meetup.Id == id);

        // meetup with provided id does not exist
        if (meetupToDelete is null)
        {
            return NotFound();
        }

        Meetups.Remove(meetupToDelete);
        return Ok(meetupToDelete);
    }

    public class Meetup
    {
        public Guid? Id { get; set; }
        public string Topic { get; set; }
        public string Place { get; set; }
        public int Duration { get; set; }
    }
}
