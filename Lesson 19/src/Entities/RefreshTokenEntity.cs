namespace Meets.WebApi.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpirationTime { get; set; }
}
