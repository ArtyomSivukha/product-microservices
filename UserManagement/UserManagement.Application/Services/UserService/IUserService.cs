using UserManagement.Domain.Users;

namespace UserManagement.Application.Services.UserService;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdAsync(Guid id);
    Task<UserModel?> GetUserByUsernameAsync(string username);
    Task<UserModel> CreateUserAsync(UserModel userModel);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task SendEmailToResetPasswordAsync(string email);
    Task<bool> IsEmailExitsAsync(string email);
    Task UpdateUserAsync(UserModel userModel);
    Task ConfirmUserAsync(string token);
    Task ResetPasswordUserAsync(UserModel userModel, string token);
    Task DeleteUserAsync(Guid id);
    
    Task ActivateUserAsync(Guid userId);
    Task DeactivateUserAsync(Guid userId);

}