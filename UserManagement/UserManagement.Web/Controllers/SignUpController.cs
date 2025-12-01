using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;


namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    public SignUpController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] SignUpUserRequest signUpUserRequest)
    {
        var existEmail = await _userService.GetUserByEmailAsync(signUpUserRequest.Email); 
        if (existEmail is not null)
        {
            return BadRequest("Email already exists");
        }
        
        var userModel = _mapper.Map<UserModel>(signUpUserRequest);
        await _userService.RegisterUserAsync(userModel, signUpUserRequest.ConfirmPassword);
        return Ok();
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        try
        {
            await _userService.ConfirmUserAsync(token);
        
            return Ok(new { message = "Email confirmed successfully. You can now login." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error confirming email: {ex.Message}");
        }
    }
    
}