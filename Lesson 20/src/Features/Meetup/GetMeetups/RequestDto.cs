namespace Meets.WebApi.Features.Meetup.GetMeetups;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class RequestDto
{
    [Required]
    public FiltersDto Filters { get; set; } = new();
    
    [Required]
    public PaginationDto Pagination { get; set; } = new();
    
    public class FiltersDto
    {
        /// <summary>The key phrase that the meetup info should contain.</summary>
        /// <example>Microsoft</example>
        public string? Search { get; set; }

        /// <summary>Minimum allowable meetup duration (in minutes).</summary>
        /// <example>120</example>
        public int? MinDuration { get; set; }
    
        /// <summary>Maximum allowable meetup duration (in minutes).</summary>
        /// <example>240</example>
        public int? MaxDuration { get; set; }
    
        /// <summary>Minimum allowable number of signed up users.</summary>
        /// <example>10</example>
        public int? MinSignedUp { get; set; }
    
        /// <summary>Maximum allowable number of signed up users.</summary>
        /// <example>30</example>
        public int? MaxSignedUp { get; set; }
    }
    
    public class PaginationDto
    {
        /// <summary>Meetups order.</summary>
        /// <example>SignedUpAscending</example>
        [Required]
        public OrderingOption Order { get; set; }
    
        /// <summary>Number of meetups per page.</summary>
        /// <example>20</example>
        [Required]
        [Range(1, 50)]
        public int PageSize { get; set; }

        /// <summary>Page number.</summary>
        /// <example>1</example>
        [Required]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; }
    
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderingOption
        {
            TopicAlphabetically,
            TopicReverseAlphabetically,
            DurationAscending,
            DurationDescending,
            SignedUpAscending,
            SignedUpDescending
        }
    }
}