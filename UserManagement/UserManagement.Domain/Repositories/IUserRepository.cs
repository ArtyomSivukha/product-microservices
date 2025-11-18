using UserManagement.Domain.Users;

namespace UserManagement.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserModel>> GetAllAsync();
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<UserModel?> GetByUsernameAsync(string username);
    Task<UserModel?> GetByEmailAsync(string email);
    // Task<bool> IsEmailConfirmedAsync(UserModel userModel);
    Task<UserModel> CreateAsync(UserModel user);
    Task UpdateAsync(UserModel user);
    Task DeleteAsync(Guid id);
}