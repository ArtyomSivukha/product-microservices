using Microsoft.AspNetCore.Identity;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<UserModel?> GetByIdAsync(Guid id)
    {
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        return userEntity;
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userEntity = await _userRepository.GetByUsernameAsync(username);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        return userEntity;
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
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
        // userModel.ConfirmPasswordHash = hasher.HashPassword(userModel, userModel.ConfirmPasswordHash);
        userModel.ConfirmPasswordHash = userModel.PasswordHash;
    
        var createdUser = await _userRepository.CreateAsync(userModel);
    
        // resultModel.PasswordHash = null;
        return createdUser;
    }

    public async Task UpdateAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }
        await _userRepository.UpdateAsync(userModel);
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}