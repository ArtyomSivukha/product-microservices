using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (model.Username == "Artyom" && model.Password == "password")
        {
            var token = _tokenService.GenerateToken(model.Username, "Admin");
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}