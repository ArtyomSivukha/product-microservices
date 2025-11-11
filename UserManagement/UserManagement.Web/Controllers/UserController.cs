using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Models;
using UserManagement.Application.Services;

namespace UserManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAuthorByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAuthorsAsync(UserModel userModel)
    {
        var createdAuthor = await _userService.CreateUserAsync(userModel);
        return Ok(createdAuthor);

    }
}