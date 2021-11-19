namespace Meets.WebApi.Features.User.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpirationTime { get; set; }
}
