using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application;
using UserManagement.Application.Security;
using UserManagement.Application.Services;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;

namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<UserModel> _passwordHasher = new();
    private readonly IMapper _mapper;

    public AuthController(ITokenService tokenService, IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        if (user.IsActive == false)
        {
            return Unauthorized("User is deactivated");
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
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
    {
        var user = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
        if (user is null)
        {
            return BadRequest("Invalid Request");
        }
       
        await _userService.SendEmailToResetPasswordAsync(user.Email);
        return Ok();
    }
    
    [HttpGet("reset-password")]
    public async Task<IActionResult> ShowResetPasswordForm([FromQuery] string token)
    {
        return Ok(new { token = token, valid = true });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDto)
    {
        try
        {
            var userModel = _mapper.Map<UserModel>(resetPasswordDto);
            await _userService.ResetPasswordUserAsync(userModel, token);
        
            return Ok(new { message = "Password reset successfully. You can now login." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error resetting password: {ex.Message}" });
        }
    }
}
