using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;
using UserManagement.Web.Security;

namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<UserModel> _passwordHasher = new();

    public AuthController(ITokenService tokenService, IUserService userService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var user = await _userService.GetByUsernameAsync(request.Username);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        try
        {
            var passwordValid = _passwordHasher
                .VerifyHashedPassword(null!, user.Password, request.Password);
            
            if (passwordValid != PasswordVerificationResult.Success)
                return Unauthorized("Invalid username or password");

            var token = _tokenService.GenerateToken(user);
            return Ok(new AuthResponse(token, new UserInfo(user.Id, user.Username, user.Role.ToString())));
        }
        catch (FormatException ex)
        {
            return StatusCode(500, "Authentication error");
        }
    }
}
