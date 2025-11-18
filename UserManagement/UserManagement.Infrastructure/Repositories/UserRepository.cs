using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var users = await _dbContext.Users.ToArrayAsync();
        return users.Select(FromEntityToModel);
    }

    public async Task<UserModel?> GetByIdAsync(Guid id)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return userEntity is null ? null : FromEntityToModel(userEntity);
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        return userEntity is null ? null : FromEntityToModel(userEntity);
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        return userEntity is null ? null : FromEntityToModel(userEntity);
    }

    // public async Task<bool> IsEmailConfirmedAsync(UserModel userModel)
    // {
    //     var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == userModel.Email);
    //     if (userEntity is null)
    //     {
    //         return false;
    //     }
    //
    //     return true;
    // }

    public async Task<UserModel> CreateAsync(UserModel user)
    {
        var userEntity = FromModelToEntity(user);
        _dbContext.Users.Add(userEntity);
        await _dbContext.SaveChangesAsync();
        return FromEntityToModel(userEntity);
    }

    public async Task UpdateAsync(UserModel user)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        userEntity.PasswordHash= user.PasswordHash;
        userEntity.IsEmailConfirmed = user.IsEmailConfirmed;
        // userEntity.Email = user.Email;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleteUserEntity = await _dbContext.Users.FindAsync(id);
        if (deleteUserEntity is null)
        {
            throw new ArgumentNullException(nameof(deleteUserEntity), $"{nameof(deleteUserEntity)} is null");
        }

        _dbContext.Users.Remove(deleteUserEntity);
        await _dbContext.SaveChangesAsync();
    }
    
    private static User FromModelToEntity(UserModel userModel) =>
        new()
        {
            Id = userModel.Id,
            Username = userModel.Username,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            PasswordHash = userModel.PasswordHash,
            ConfirmPasswordHash = userModel.ConfirmPasswordHash,
            Role = Enum.TryParse<UserRole>(userModel.Role, true, out var role) ? role : UserRole.User
        };

    private static UserModel FromEntityToModel(User user) =>
        new()
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            ConfirmPasswordHash = user.ConfirmPasswordHash,
            Role = user.Role.ToString()
        };
}