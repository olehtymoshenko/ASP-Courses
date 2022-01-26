namespace Meets.WebApi.Features.Meetup.RegisterNewMeetup;

using System.ComponentModel.DataAnnotations;

public class RegisterMeetupDto
{
    /// <summary>Topic discussed on meetup.</summary>
    /// <example>Microsoft naming issues.</example>
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\-]*$")]
    public string Topic { get; set; } = string.Empty;
    
    /// <summary>Meetup location.</summary>
    /// <example>Oslo</example>
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\d]*")]
    public string Place { get; set; } = string.Empty;
    
    /// <summary>Meetup duration in minutes.</summary>
    /// <example>180</example>
    [Required]
    [Range(30, 300)]
    public int Duration { get; set; }
}

public class RegisteredMeetupDto
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
}