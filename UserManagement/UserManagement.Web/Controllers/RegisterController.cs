using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Models;
using UserManagement.Application.Services;


namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly IUserService _userService;

    public RegisterController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserModel userModel)
    {
        var registerAuthor = await _userService.RegisterUserAsync(userModel);
        return Ok(registerAuthor);
    }
}