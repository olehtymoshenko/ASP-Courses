namespace Meets.WebApi.Features.User.AuthenticateUser;

using System.ComponentModel.DataAnnotations;

public class AuthenticateUserDto
{
    /// <summary>Username for authentication.</summary>
    /// <example>tony_lore</example>
    [Required]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>Password for authentication.</summary>
    /// <example>password123</example>
    [Required]
    public string Password { get; set; } = string.Empty;
}


public class TokenPairDto
{
    /// <summary>JWT Access Token.</summary>
    /// <example>header.payload.signature</example>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>JWT Refresh Token.</summary>
    /// <example>header.payload.signature</example>
    public string RefreshToken { get; set; } = string.Empty;
}