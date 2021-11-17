namespace Meets.WebApi.Meetup;

using System.ComponentModel.DataAnnotations;

public class ReadMeetupDto
{
    /// <summary>Meetup id.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; set; }
    
    /// <summary>Topic discussed on meetup.</summary>
    /// <example>Microsoft naming issues.</example>
    public string Topic { get; set; }
    
    /// <summary>Meetup location.</summary>
    /// <example>Oslo</example>
    public string Place { get; set; }
    
    /// <summary>Meetup duration in minutes.</summary>
    /// <example>180</example>
    public int Duration { get; set; }
    
    /// <summary>Number of users signed up for the meetup.</summary>
    /// <example>42</example>
    public int SignedUp { get; set; }
}

public class CreateMeetupDto
{
    /// <summary>Topic discussed on meetup.</summary>
    /// <example>Microsoft naming issues.</example>
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\-]*$")]
    public string Topic { get; set; }
    
    /// <summary>Meetup location.</summary>
    /// <example>Oslo</example>
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\d]*")]
    public string Place { get; set; }
    
    /// <summary>Meetup duration in minutes.</summary>
    /// <example>180</example>
    [Required]
    [Range(30, 300)]
    public int Duration { get; set; }
}

public class UpdateMeetupDto
{
    /// <summary>Topic discussed on meetup.</summary>
    /// <example>Microsoft naming issues.</example>
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.-–—]*$")]
    public string Topic { get; set; }
    
    /// <summary>Meetup location.</summary>
    /// <example>Oslo</example>
    [Required]
    [MaxLength(25)]
    [RegularExpression(@"^[\w\s]*")]
    public string Place { get; set; }
    
    /// <summary>Meetup duration in minutes.</summary>
    /// <example>180</example>
    [Required]
    [Range(30, 300)]
    public int Duration { get; set; }
}
