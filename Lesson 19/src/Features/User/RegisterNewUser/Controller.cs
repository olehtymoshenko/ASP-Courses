namespace Meets.WebApi.Features.User.RegisterNewUser;

using System.Net.Mime;
using AutoMapper;
using BCrypt.Net;
using Meets.WebApi.Entities;
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

    public Controller(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>Register a new user.</summary>
    /// <param name="registerDto">User registration information.</param>
    /// <response code="200">Newly registered user.</response>
    /// <response code="409">Failed to register a user: username already taken.</response>
    [HttpPost("/users/register")]
    [ProducesResponseType(typeof(RegisteredUserDto), StatusCodes.Status200OK)]
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

        var readDto = _mapper.Map<RegisteredUserDto>(newUser);
        return Ok(readDto);
    }
}