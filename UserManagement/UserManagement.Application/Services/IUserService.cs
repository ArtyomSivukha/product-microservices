using UserManagement.Application.Models;

namespace UserManagement.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllAsync();
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<UserModel> GetByEmailAsync(string email);
    Task<UserModel> CreateUserAsync(UserModel userModel);
    Task UpdateAsync(UserModel userModel);
    Task DeleteAsync(Guid id);
}