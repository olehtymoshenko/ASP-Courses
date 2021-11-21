namespace Meets.WebApi.Features.User;

using System.Net.Mime;
using AutoMapper;
using BCrypt.Net;
using Meets.WebApi;
using Meets.WebApi.Features.User.DataTransferObjects;
using Meets.WebApi.Features.User.Entities;
using Meets.WebApi.Features.User.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/users")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenHelper _tokenHelper;
    private readonly IMapper _mapper;

    public UserController(
        DatabaseContext context,
        IConfiguration configuration,
        JwtTokenHelper tokenHelper,
        IMapper mapper)
    {
        _context = context;
        _configuration = configuration;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
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

        var readDto = _mapper.Map<ReadUserDto>(currentUser);
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

        var newUser = _mapper.Map<UserEntity>(registerDto);
        newUser.Password = BCrypt.HashPassword(registerDto.Password);
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var readDto = _mapper.Map<ReadUserDto>(newUser);
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
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == authenticateDto.Username);
        if (user is null)
        {
            return NotFound();
        }
        if (!BCrypt.Verify(authenticateDto.Password, user.Password))
        {
            return Conflict("Incorrect password.");
        }

        var refreshTokenLifetime = int.Parse(_configuration["JwtAuth:RefreshTokenLifetime"]);
        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpirationTime = DateTime.UtcNow.AddDays(refreshTokenLifetime)
        };
        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var tokenPair = _tokenHelper.IssueTokenPair(user.Id, refreshTokenEntity.Id);
        var tokenPairDto = _mapper.Map<TokenPairDto>(tokenPair);
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
        var refreshTokenClaims = _tokenHelper.ParseToken(refreshToken);
        if (refreshTokenClaims is null)
        {
            return BadRequest("Invalid refresh token was provided.");
        }
        
        var refreshTokenId = Guid.Parse(refreshTokenClaims["jti"]);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshTokenId);
        if (refreshTokenEntity is null)
        {
            return Conflict("Provided refresh token has already been used.");
        }

        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var userId = Guid.Parse(refreshTokenClaims["sub"]);
        var refreshTokenLifetime = int.Parse(_configuration["JwtAuth:RefreshTokenLifetime"]);
        var newRefreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ExpirationTime = DateTime.UtcNow.AddDays(refreshTokenLifetime)
        };
        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        var tokenPair = _tokenHelper.IssueTokenPair(userId, refreshTokenEntity.Id);
        var tokenPairDto = _mapper.Map<TokenPairDto>(tokenPair);
        return Ok(tokenPairDto);
    }
}
