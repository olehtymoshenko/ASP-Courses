namespace Meets.WebApi.Features.Meetup.Entities;

using Meets.WebApi.Features.User.Entities;

public class MeetupEntity
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Duration { get; set; }
    
    public ICollection<UserEntity>? SignedUpUsers { get; set; }
}
