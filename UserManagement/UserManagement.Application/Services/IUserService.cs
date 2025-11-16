using UserManagement.Domain.Users;

namespace UserManagement.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllAsync();
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<UserModel?> GetByUsernameAsync(string username);
    Task<UserModel> CreateUserAsync(UserModel userModel);
    Task<UserModel?> GetByEmailAsync(string email);
    Task UpdateAsync(UserModel userModel);
    Task DeleteAsync(Guid id);

}