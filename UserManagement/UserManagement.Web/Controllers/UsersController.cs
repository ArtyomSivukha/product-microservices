using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;

namespace UserManagement.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(_mapper.Map<IEnumerable<UserAdminResponse>>(users));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAuthorByIdAsync(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(_mapper.Map<UserResponse>(user));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAuthorsAsync(UserModel userModel)
    {
        var createdAuthor = await _userService.CreateUserAsync(userModel);
        return Ok(_mapper.Map<UserAdminResponse>(createdAuthor));
    }
    
    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        await _userService.ActivateUserAsync(id);
        return Ok();
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        await _userService.DeactivateUserAsync(id);
        return Ok();
    }
}