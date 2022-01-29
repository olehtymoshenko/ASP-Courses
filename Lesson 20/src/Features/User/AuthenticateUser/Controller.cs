namespace Meets.WebApi.Features.User.AuthenticateUser;

using System.Net.Mime;
using AutoMapper;
using BCrypt.Net;
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
    
    /// <summary>Authenticate the user.</summary>
    /// <param name="authenticateDto">User authentication information.</param>
    /// <response code="200">Authentication token pair for specified user credentials.</response>
    /// <response code="404">User with specified username does not exist.</response>
    /// <response code="409">Incorrect password was specified.</response>
    [HttpPost("/users/authenticate")]
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
}