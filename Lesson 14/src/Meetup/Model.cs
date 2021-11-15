namespace Meets.WebApi.Meetup;

using System.ComponentModel.DataAnnotations.Schema;

[Table("meetups")]
public class MeetupEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("topic")]
    public string Topic { get; set; }
    
    [Column("place")]
    public string Place { get; set; }
    
    [Column("duration")]
    public int Duration { get; set; }
}
