namespace Meets.WebApi.Features.User.RegisterNewUser;

using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    /// <summary>User display name.</summary>
    /// <example>Tony Lore</example>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[\w\s]*$")]
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    [Required]
    [MaxLength(30)]
    [RegularExpression(@"^[\w\s\d]*$")]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>Password for authentication.</summary>
    /// <example>password123</example>
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    [RegularExpression(@"^[\w\s\d]*$")]
    public string Password { get; set; } = string.Empty;
}

public class RegisteredUserDto
{
    /// <summary>User identifier.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; set; }
    
    /// <summary>User display name.</summary>
    /// <example>Tony Lore</example>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    public string Username { get; set; } = string.Empty;
}