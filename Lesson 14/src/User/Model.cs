namespace Meets.WebApi.User;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("users")]
[Index(nameof(Username), IsUnique = true)]
public class UserEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("display_name")]
    public string DisplayName { get; set; }
    
    [Column("username")]
    public string Username { get; set; }
    
    [Column("password")]
    public string Password { get; set; }
}

[Table("refresh_tokens")]
public class RefreshTokenEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("expiration_time")]
    public DateTime ExpirationTime { get; set; }
}
