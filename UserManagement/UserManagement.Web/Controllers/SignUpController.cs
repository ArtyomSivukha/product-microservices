using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services;
using UserManagement.Domain.Users;


namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly IUserService _userService;

    public SignUpController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserModel userModel)
    {
        var registerAuthor = await _userService.CreateUserAsync(userModel);
        return Ok(registerAuthor);
    }
}