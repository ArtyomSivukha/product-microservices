using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services;
using UserManagement.Domain.Users;
using UserManagement.Web.Email;
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
    private readonly IEmailSender _emailSender;

    public AuthController(ITokenService tokenService, IUserService userService, IEmailSender emailSender)
    {
        _userService = userService;
        _tokenService = tokenService;
        _emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        if (user.IsEmailConfirmed == false)
        {
            return Unauthorized("Email is not confirmed");
        }

        try
        {
            var passwordValid = _passwordHasher
                .VerifyHashedPassword(null!, user.PasswordHash, request.Password);
            
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
