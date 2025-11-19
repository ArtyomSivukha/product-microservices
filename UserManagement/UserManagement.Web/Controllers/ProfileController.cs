using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services;
using UserManagement.Web.Model;

namespace UserManagement.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserIdFromToken();
        
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        var userId = GetUserIdFromToken();
        
        var currentUser = await _userService.GetUserByIdAsync(userId);
        if (currentUser == null)
            return NotFound();

        if (request.FirstName != null)
            currentUser.FirstName = request.FirstName;
        
        if (request.LastName != null)
            currentUser.LastName = request.LastName;
            
        await _userService.UpdateUserAsync(currentUser);

        return Ok(currentUser);
    }

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim);
    }
}