using Microsoft.AspNetCore.Identity;
using UserManagement.Application.LinkURI;
using UserManagement.Application.Security;
using UserManagement.Application.Services.EmailConfirmService;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;

namespace UserManagement.Application.Services.UserService;

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

        userModel.IsEmailConfirmed = false;
        userModel.IsActive = false;
        
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

    public async Task ConfirmUserAsync(string token)
    {
        var userId = await _emailConfirmService.GetUserIdByTokenAsync(token);
        if (userId is null)
        {
            throw new Exception("Invalid token");
        }

        var user = await GetUserByIdAsync(userId.Value);
        if (user is null)
        {
            throw new Exception("User not found");
        }

        if (user.IsEmailConfirmed)
        {
            throw new Exception("Email is already confirmed");
        }

        user.IsEmailConfirmed = true;
        user.IsActive = true;
        await UpdateUserAsync(user);

        await _emailConfirmService.DeleteByUserIdAsync(user.Id);
    }
    
    public async Task SendEmailToResetPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user), $"{nameof(user)} is null");
        }

        var token = _tokenService.GenerateEmailConfirmationToken(user);

        await _emailConfirmService.CreateEmailConfirmAsync(user.Id, token);

        user.IsActive = false;

        await UpdateUserAsync(user);

        var resetLink = _linkService.CreatePasswordResetLink(token);

        var message = new Message(
            [user.Email],
            "Reset Your Password",
            $"Click here to reset your password: {resetLink}"
        );

        await _emailSender.SendEmailAsync(message);
    }
    
    public async Task ResetPasswordUserAsync(UserModel userModel, string token)
    {
        var userId = await _emailConfirmService.GetUserIdByTokenAsync(token);
        if (userId is null)
        {
            throw new Exception("Invalid token");
        }

        var user = await GetUserByIdAsync(userId.Value);
        if (user is null)
        {
            throw new Exception("User not found");
        }

        if (userModel.PasswordHash != userModel.ConfirmPasswordHash)
        {
            throw new Exception("Passwords do not match");
        }

        if (user.PasswordHash == userModel.PasswordHash)
        {
            throw new Exception("Enter new password");
        }

        var hasher = new PasswordHasher<UserModel>();
        var passwordVerification = hasher.VerifyHashedPassword(user, user.PasswordHash, userModel.PasswordHash);
        if (passwordVerification == PasswordVerificationResult.Success)
        {
            throw new Exception("New password must be different from current password");
        }

        user.PasswordHash = hasher.HashPassword(userModel, userModel.PasswordHash);
        user.ConfirmPasswordHash = user.PasswordHash;
        user.IsActive = true;
        await UpdateUserAsync(user);

        await _emailConfirmService.DeleteByUserIdAsync(user.Id);
    }

    public Task DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task ActivateUserAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new Exception("User not found");
        }
        
        user.IsActive = true;

        await UpdateUserAsync(user);
    }

    public async Task DeactivateUserAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new Exception("User not found");
        }

        user.IsActive = false;

        await UpdateUserAsync(user);
    }
}