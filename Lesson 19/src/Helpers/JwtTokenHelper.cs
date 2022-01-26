namespace Meets.WebApi.Helpers;

using System.IdentityModel.Tokens.Jwt;
using Meets.WebApi.Extensions;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenHelper
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IConfiguration _configuration;

    public JwtTokenHelper(IConfiguration configuration)
    {
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenHandler.InboundClaimTypeMap.Clear();
        _tokenHandler.OutboundClaimTypeMap.Clear();

        _configuration = configuration;
    }
    
    public IDictionary<string, string>? ParseToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _configuration.GetAuthSecret(),
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
            _configuration.GetAccessTokenLifetime());
        
        var refreshToken = IssueToken(
            new Dictionary<string, object>
            {
                {"sub", userId},
                {"jti", refreshTokenId}
            },
            _configuration.GetRefreshTokenLifetime());

        return new TokenPair(accessToken, refreshToken);
    }

    private string IssueToken(IDictionary<string, object> claims, TimeSpan lifetime)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials =
                new SigningCredentials(_configuration.GetAuthSecret(), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenObject = _tokenHandler.CreateToken(descriptor);
        var encodedToken = _tokenHandler.WriteToken(tokenObject);

        return encodedToken;
    }
}
