namespace Meets.WebApi.Features.Meetup.GetAllMeetups;

public class MeetupDto
{
    /// <summary>Meetup id.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; set; }
    
    /// <summary>Topic discussed on meetup.</summary>
    /// <example>Microsoft naming issues.</example>
    public string Topic { get; set; } = string.Empty;
    
    /// <summary>Meetup location.</summary>
    /// <example>Oslo</example>
    public string Place { get; set; } = string.Empty;
    
    /// <summary>Meetup duration in minutes.</summary>
    /// <example>180</example>
    public int Duration { get; set; }
    
    /// <summary>Number of users signed up for the meetup.</summary>
    /// <example>42</example>
    public int SignedUp { get; set; }
}