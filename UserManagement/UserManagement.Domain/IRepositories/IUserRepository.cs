using UserManagement.Domain.Entities;

namespace UserManagement.Domain.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<User?>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<User?> CreateAsync(User? user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}