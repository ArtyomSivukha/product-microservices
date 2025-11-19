using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using UserManagement.Application.LinkURI;
using UserManagement.Application.Security;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly ILinkService _linkService;
    private readonly IEmailConfirmService _emailConfirmService;
    public UserService(IUserRepository userRepository, ITokenService tokenService, IEmailSender emailSender,
        ILinkService linkService, IEmailConfirmService emailConfirmService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _linkService = linkService;
        _emailConfirmService = emailConfirmService;
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<UserModel?> GetUserByIdAsync(Guid id)
    {
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        return userEntity;
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        var userEntity = await _userRepository.GetByUsernameAsync(username);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        
        return userEntity;
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<bool> IsEmailExitsAsync(string email)
    {
        var userEntity = await _userRepository.GetByEmailAsync(email);
        return userEntity is not null;
    }

    public async Task<UserModel> CreateUserAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }

        var hasher = new PasswordHasher<UserModel>();
        userModel.PasswordHash = hasher.HashPassword(userModel, userModel.PasswordHash);
        userModel.ConfirmPasswordHash = userModel.PasswordHash;
    
        var createdUser = await _userRepository.CreateAsync(userModel);
          
        var token = _tokenService.GenerateEmailConfirmationToken(createdUser);
        
        await _emailConfirmService.CreateEmailConfirmAsync(createdUser.Id, token);
        
        var verificationLink = _linkService.CreateVerificationLink(token);
    
        var message = new Message(
            [userModel.Email], 
            "Email Confirmation", 
            $"Please confirm your email by clicking here: {verificationLink}"
        );
    
        await _emailSender.SendEmailAsync(message);
        
        // resultModel.PasswordHash = null;
        return createdUser;
    }

    public async Task UpdateUserAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }
        await _userRepository.UpdateAsync(userModel);
    }

    public Task DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}