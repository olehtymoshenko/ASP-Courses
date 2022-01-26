namespace Meets.WebApi.Features.User.RefreshTokenPair;

using System.Net.Mime;
using AutoMapper;
using Meets.WebApi.Entities;
using Meets.WebApi.Helpers;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Tags("Users")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class Controller : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenHelper _tokenHelper;

    public Controller(DatabaseContext context, IMapper mapper, IConfiguration configuration, JwtTokenHelper tokenHelper)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _tokenHelper = tokenHelper;
    }
    
    /// <summary>Refresh token pair.</summary>
    /// <param name="refreshToken">Refresh token.</param>
    /// <response code="200">A new token pair.</response>
    /// <response code="400">Invalid refresh token was provided.</response>
    /// <response code="409">Provided refresh token has already been used.</response>
    [HttpPost("/users/refresh")]
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