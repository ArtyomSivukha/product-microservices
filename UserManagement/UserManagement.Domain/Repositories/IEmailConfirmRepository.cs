namespace UserManagement.Domain.Repositories;

public interface IEmailConfirmRepository
{
    Task CreateAsync(Guid userId, string token);
    Task<Guid?> GetUserIdByTokenAsync(string token);
    Task DeleteByUserIdAsync(Guid userId);
}