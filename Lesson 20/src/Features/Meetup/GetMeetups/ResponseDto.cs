namespace Meets.WebApi.Features.Meetup.GetMeetups;

public class ResponseDto
{
    public PaginationDto Pagination { get; init; } = null!;

    public ICollection<MeetupDto> Meetups { get; init; } = null!;
    
    public class PaginationDto
    {
        /// <summary>Number of pages that satisfy filtering criteria.</summary>
        /// <example>7</example>
        public int PagesCount { get; init; }
        
        /// <summary>Total number of meetups that satisfy filtering criteria.</summary>
        /// <example>137</example>
        public int MeetupsCount { get; init; }
    }
    
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
}