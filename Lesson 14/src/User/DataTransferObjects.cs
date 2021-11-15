namespace Meets.WebApi.User;

using System.ComponentModel.DataAnnotations;

public class ReadUserDto
{
    /// <summary>User identifier.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; set; }
    
    /// <summary>User display name.</summary>
    /// <example>Tony Lore</example>
    public string DisplayName { get; set; }
    
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    public string Username { get; set; }
}

public class RegisterUserDto
{
    /// <summary>User display name.</summary>
    /// <example>Tony Lore</example>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[\w\s]*$")]
    public string DisplayName { get; set; }
    
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    [Required]
    [MaxLength(30)]
    [RegularExpression(@"^[\w\s\d]*$")]
    public string Username { get; set; }
    
    /// <summary>Password for authentication.</summary>
    /// <example>password123</example>
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    [RegularExpression(@"^[\w\s\d]*$")]
    public string Password { get; set; }
}

public class AuthenticateUserDto
{
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    [Required]
    public string Username { get; set; }
    
    /// <summary>Password for authentication.</summary>
    /// <example>password123</example>
    [Required]
    public string Password { get; set; }
}

public class TokenPairDto
{
    /// <summary>JWT Access Token.</summary>
    /// <example>header.payload.signature</example>
    public string AccessToken { get; set; }
    
    /// <summary>JWT Refresh Token.</summary>
    /// <example>header.payload.signature</example>
    public string RefreshToken { get; set; }
}
