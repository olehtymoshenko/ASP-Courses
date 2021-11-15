namespace Meets.WebApi.Helpers;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenHelper
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly TimeSpan _accessTokenLifetime;
    private readonly TimeSpan _refreshTokenLifetime;

    public JwtTokenHelper(IConfiguration configuration)
    {
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenHandler.InboundClaimTypeMap.Clear();
        _tokenHandler.OutboundClaimTypeMap.Clear();

        var jwtSecret = Encoding.ASCII.GetBytes(configuration["JwtAuth:Secret"]);
        _securityKey = new SymmetricSecurityKey(jwtSecret);

        var accessTokenLifetimeInMinutes = int.Parse(configuration["JwtAuth:AccessTokenLifetime"]);
        _accessTokenLifetime = TimeSpan.FromMinutes(accessTokenLifetimeInMinutes);
        
        var refreshTokenLifetimeInDays = int.Parse(configuration["JwtAuth:RefreshTokenLifetime"]);
        _refreshTokenLifetime = TimeSpan.FromMinutes(refreshTokenLifetimeInDays);
    }
    
    public IDictionary<string, string> ParseToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidateAudience = false,
            ValidateIssuer = false,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var claimsPrincipal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return claimsPrincipal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public TokenPair IssueTokenPair(Guid userId, Guid refreshTokenId)
    {
        var accessToken = IssueToken(
            new Dictionary<string, object>
            {
                {"sub", userId}
            },
            _accessTokenLifetime);
        
        var refreshToken = IssueToken(
            new Dictionary<string, object>
            {
                {"sub", userId},
                {"jti", refreshTokenId}
            },
            _refreshTokenLifetime);

        return new TokenPair(accessToken, refreshToken);
    }

    private string IssueToken(IDictionary<string, object> claims, TimeSpan lifetime)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenObject = _tokenHandler.CreateToken(descriptor);
        var encodedToken = _tokenHandler.WriteToken(tokenObject);

        return encodedToken;
    }

    public record TokenPair(string AccessToken, string RefreshToken);
}
