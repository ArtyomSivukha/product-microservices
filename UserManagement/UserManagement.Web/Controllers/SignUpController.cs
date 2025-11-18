using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using UserManagement.Application.Services;
using UserManagement.Domain.Users;
using UserManagement.Web.Email;
using UserManagement.Web.Model;
using UserManagement.Web.Security;


namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly ITokenService _tokenService;
    
    public SignUpController(IUserService userService, IMapper mapper, IEmailSender emailSender, ITokenService tokenService)
    {
        _userService = userService;
        _mapper = mapper;
        _emailSender = emailSender;
        _tokenService = tokenService;
    }

    [HttpGet ("email")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var message = new Message(new string[] { "usermanagement@gmail.com" }, "Test email", "This is the content from our email.");
            _emailSender.SendEmailAsync(message);
        
            return Ok(new { 
                message = "Email sent successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] SignUpUserRequest signUpUserRequest)
    {
        var existEmail = await _userService.GetByEmailAsync(signUpUserRequest.Email); // add exists method 
        if (existEmail is not null)
        {
            return BadRequest("Email already exists");
        }
        
        var userModel = _mapper.Map<UserModel>(signUpUserRequest);
        
        var registerAuthor = await _userService.CreateUserAsync(userModel);
        
        var token = _tokenService.GenerateToken(registerAuthor);
        var param = new Dictionary<string, string?>
        {
            { "token", token },
            { "email", userModel.Email }
        };
        
        var callback = QueryHelpers.AddQueryString(signUpUserRequest.ClientUri, param);
        
        var message = new Message(
            [userModel.Email], "EmailConfirmationToken", callback
        );
        
        await _emailSender.SendEmailAsync(message);

        return Ok(registerAuthor);
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user is null)
            {
                return BadRequest("Invalid confirmation link");
            }
//validation
            if (user.IsEmailConfirmed)
            {

                return Ok(new { message = "Email is already confirmed" });
            }
            
            user.IsEmailConfirmed = true;
            await _userService.UpdateAsync(user);
            
            return Ok(new { message = "Email confirmed successfully. You can now login." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error confirming email: {ex.Message}");
        }
    }
    
}