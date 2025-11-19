namespace UserManagement.Application.Services;

public interface IEmailConfirmService
{
    Task CreateEmailConfirmAsync(Guid userId, string token);
    Task<Guid?> GetUserIdByTokenAsync(string token);
    Task DeleteByUserIdAsync(Guid userId);
}