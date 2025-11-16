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

    public Task<UserModel?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel> CreateUserAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }

        var hasher = new PasswordHasher<UserModel>();
        userModel.Password = hasher.HashPassword(userModel, userModel.Password);
    
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