using UserManagement.Domain.Users;

namespace UserManagement.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdAsync(Guid id);
    Task<UserModel?> GetUserByUsernameAsync(string username);
    Task<UserModel> CreateUserAsync(UserModel userModel);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailExitsAsync(string email);
    Task UpdateUserAsync(UserModel userModel);
    Task DeleteUserAsync(Guid id);

}