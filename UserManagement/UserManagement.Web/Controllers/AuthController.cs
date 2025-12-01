using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;
using UserManagement.Web.Security;
using AccessViolationException = UserManagement.Application.Exceptions.AccessViolationException;

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
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            throw new AccessViolationException("Invalid email");
        }

        if (user.IsActive == false)
        {
            throw new AccessViolationException("User is deactivated");
        }
        
        if (user.IsEmailConfirmed == false)
        {
            throw new AccessViolationException("Email is not confirmed");
        }

        try
        {
            var passwordValid = _passwordHasher
                .VerifyHashedPassword(null!, user.Password, request.Password);

            if (passwordValid != PasswordVerificationResult.Success)
                throw new AccessViolationException("Invalid password");

            var token = _tokenService.GenerateToken(user);
            return Ok(new AuthResponse(token, new UserInfoResponse(user.Id, user.Username, user.Role.ToString())));
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Authentication error", ex);
        }
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
    {
        var user = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
        _ = user ?? throw new ArgumentNullException(nameof(user), "Invalid email");
       
        await _userService.SendEmailToResetPasswordAsync(user.Email);
        return Ok();
    }
    
    [HttpGet("reset-password")]
    public async Task<IActionResult> ShowResetPasswordForm([FromQuery] string token)
    {
        return Ok(new { token });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDto)
    {
        try
        {
            await _userService.ResetPasswordUserAsync(
                resetPasswordDto.Password,
                resetPasswordDto.ConfirmPassword,
                token);
        
            return Ok(new { message = "Password reset successfully. You can now login." });
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error resetting password: {ex.Message}");
        }
    }
}
