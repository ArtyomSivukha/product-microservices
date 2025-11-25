namespace UserManagement.Application.Services.EmailConfirmService;

public interface IEmailConfirmService
{
    Task CreateEmailConfirmAsync(Guid userId, string token);
    Task<Guid?> GetUserIdByTokenAsync(string token);
    Task DeleteTokenByUserIdAsync(Guid userId);
}