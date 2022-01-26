namespace Meets.WebApi.Features.User.RefreshTokenPair;

public class TokenPairDto
{
    /// <summary>JWT Access Token.</summary>
    /// <example>header.payload.signature</example>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>JWT Refresh Token.</summary>
    /// <example>header.payload.signature</example>
    public string RefreshToken { get; set; } = string.Empty;
}