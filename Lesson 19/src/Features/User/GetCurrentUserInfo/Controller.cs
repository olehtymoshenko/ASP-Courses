namespace Meets.WebApi.Features.User.GetCurrentUserInfo;

using System.Net.Mime;
using AutoMapper;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Authorization;
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
    
    /// <summary>Get information about the current user.</summary>
    /// <response code="200">Current user information.</response>
    [HttpGet("/users/who-am-i")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserInfoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var subClaim = User.Claims.Single(claim => claim.Type == "sub");
        var currentUserId = Guid.Parse(subClaim.Value);

        var currentUser = await _context.Users.SingleAsync(user => user.Id == currentUserId);

        var readDto = _mapper.Map<CurrentUserInfoDto>(currentUser);
        return Ok(readDto);
    }
}