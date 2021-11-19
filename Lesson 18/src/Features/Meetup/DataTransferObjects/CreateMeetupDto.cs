namespace Meets.WebApi.Features.Meetup.DataTransferObjects;

using System.ComponentModel.DataAnnotations;

public class CreateMeetupDto
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
