using Microsoft.AspNetCore.Identity;
using UserManagement.Application.Models;
using UserManagement.Domain.Enums;
using UserManagement.Domain.IRepositories;

using EntityUser = UserManagement.Domain.Entities.User;

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
        var users = await _userRepository.GetAllAsync();
        return users.Select(FromEntityToModel);
    }

    public async Task<UserModel?> GetByIdAsync(Guid id)
    {
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        return FromEntityToModel(userEntity);
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userEntity = await _userRepository.GetByUsernameAsync(username);
    
        if (userEntity is null)
            return null;
    
        return FromEntityToModel(userEntity);
    }

    public Task<UserModel> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel> CreateUserAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }

        var userEntity = FromModelToEntity(userModel);
        var createdEntity = await _userRepository.CreateAsync(userEntity);
        return FromEntityToModel(createdEntity);
    }

    public async Task UpdateAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }
        
        var userEntity = FromModelToEntity(userModel);
        await _userRepository.UpdateAsync(userEntity);
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel> RegisterUserAsync(UserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel), $"{nameof(userModel)} is null");
        }

        var userEntity = FromModelToEntity(userModel);

        var hasher = new PasswordHasher<UserModel>();
        userEntity.PasswordHash = hasher.HashPassword(userModel, userModel.Password);
    
        var registerEntity = await _userRepository.CreateAsync(userEntity);
    
        var resultModel = FromEntityToModel(registerEntity);
        // resultModel.PasswordHash = null;
    
        return resultModel;
    }


    private static EntityUser FromModelToEntity(UserModel userModel) =>
        new()
        {
            Id = userModel.Id,
            Username = userModel.Username,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            PasswordHash = userModel.Password,
            Role = Enum.TryParse<UserRole>(userModel.Role, true, out var role) ? role : UserRole.User
        };

    private static UserModel FromEntityToModel(EntityUser user) =>
        new()
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.PasswordHash,
            Role = user.Role.ToString()
        };
}