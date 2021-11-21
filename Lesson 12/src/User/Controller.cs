namespace Meets.WebApi.User;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("/users")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public UserController(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>Get information about the current user.</summary>
    /// <response code="200">Current user information.</response>
    [HttpGet("who-am-i")]
    [Authorize]
    [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var subClaim = User.Claims.Single(claim => claim.Type == "sub");
        var currentUserId = Guid.Parse(subClaim.Value);

        var currentUser = await _context.Users.SingleAsync(user => user.Id == currentUserId);

        var readDto = new ReadUserDto
        {
            Id = currentUser.Id,
            DisplayName = currentUser.DisplayName,
            Username = currentUser.Username
        };
        return Ok(readDto);
    }

    /// <summary>Register a new user.</summary>
    /// <param name="registerDto">User registration information.</param>
    /// <response code="200">Newly registered user.</response>
    /// <response code="409">Failed to register a user: username already taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserDto registerDto)
    {
        var usernameTaken = await _context.Users.AnyAsync(user => user.Username == registerDto.Username);
        if (usernameTaken)
        {
            return Conflict("Username already taken.");
        }
        
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = registerDto.DisplayName,
            Username = registerDto.Username,
            Password = BCrypt.HashPassword(registerDto.Password)
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var readDto = new ReadUserDto
        {
            Id = newUser.Id,
            DisplayName = newUser.DisplayName,
            Username = newUser.Username
        };
        return Ok(readDto);
    }
    
    /// <summary>Authenticate the user.</summary>
    /// <param name="authenticateDto">User authentication information.</param>
    /// <response code="200">Authentication token pair for specified user credentials.</response>
    /// <response code="404">User with specified username does not exist.</response>
    /// <response code="409">Incorrect password was specified.</response>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(TokenPairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserDto authenticateDto)
    {
        // Filter out bad requests
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == authenticateDto.Username);
        if (user is null)
        {
            return NotFound();
        }
        if (!BCrypt.Verify(authenticateDto.Password, user.Password))
        {
            return Conflict("Incorrect password.");
        }

        // Prepare token info
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecret = Encoding.ASCII.GetBytes(_configuration["JwtAuth:Secret"]);
        var accessTokenLifetime = int.Parse(_configuration["JwtAuth:AccessTokenLifetime"]);
        var refreshTokenLifetime = int.Parse(_configuration["JwtAuth:RefreshTokenLifetime"]);
        
        // Issue access token
        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim("sub", user.Id.ToString())}),
            Expires = DateTime.UtcNow.AddMinutes(accessTokenLifetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        var encodedAccessToken = tokenHandler.WriteToken(accessToken);
        
        // Save refresh token info
        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpirationTime = DateTime.UtcNow.AddDays(refreshTokenLifetime)
        };
        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();
        
        // Issue refresh token
        var refreshTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("jti", refreshTokenEntity.Id.ToString())
            }),
            Expires = refreshTokenEntity.ExpirationTime,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);
        var encodedRefreshToken = tokenHandler.WriteToken(refreshToken);
        

        // Return token pair back to user
        var tokenPairDto = new TokenPairDto
        {
            AccessToken = encodedAccessToken,
            RefreshToken = encodedRefreshToken
        };
        return Ok(tokenPairDto);
    }
    
    /// <summary>Refresh token pair.</summary>
    /// <param name="refreshToken">Refresh token.</param>
    /// <response code="200">A new token pair.</response>
    /// <response code="400">Invalid refresh token was provided.</response>
    /// <response code="409">Provided refresh token has already been used.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenPairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RefreshTokenPair([FromBody] string refreshToken)
    {
        var jwtSecret = Encoding.ASCII.GetBytes(_configuration["JwtAuth:Secret"]);
        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecret),

            ValidateAudience = false,
            ValidateIssuer = false,

            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear();
        tokenHandler.OutboundClaimTypeMap.Clear();

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out _);

            // Check if token has already been used 
            var jtiClaim = principal.Claims.Single(claim => claim.Type == "jti");
            var refreshTokenId = Guid.Parse(jtiClaim.Value);
            var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshTokenId);
            if (refreshTokenEntity is null)
            {
                // The token is either fake or has already been used
                return Conflict("Provided refresh token has already been used.");
            }

            // Remove a token from the database so that it can no longer be used
            _context.RefreshTokens.Remove(refreshTokenEntity);
            await _context.SaveChangesAsync();

            // Prepare token info
            var userId = principal.Claims.Single(claim => claim.Type == "sub").Value;
            var accessTokenLifetime = int.Parse(_configuration["JwtAuth:AccessTokenLifetime"]);
            var refreshTokenLifetime = int.Parse(_configuration["JwtAuth:RefreshTokenLifetime"]);

            // Issue new access token
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("sub", userId)}),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var newAccessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            var encodedNewAccessToken = tokenHandler.WriteToken(newAccessToken);

            // Save refresh token info
            var newRefreshTokenEntity = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                ExpirationTime = DateTime.UtcNow.AddDays(refreshTokenLifetime)
            };
            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            // Issue new refresh token
            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", userId),
                    new Claim("jti", refreshTokenEntity.Id.ToString())
                }),
                Expires = refreshTokenEntity.ExpirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var newRefreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);
            var encodedNewRefreshToken = tokenHandler.WriteToken(newRefreshToken);

            // Return new token pair back to user
            var tokenPairDto = new TokenPairDto
            {
                AccessToken = encodedNewAccessToken,
                RefreshToken = encodedNewRefreshToken
            };
            return Ok(tokenPairDto);
        }
        catch (Exception)
        {
            return BadRequest("Invalid refresh token was provided.");
        }
    }
}
