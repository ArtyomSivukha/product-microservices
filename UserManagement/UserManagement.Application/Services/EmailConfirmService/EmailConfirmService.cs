using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services.EmailConfirmService;

public class EmailConfirmService : IEmailConfirmService
{
    private readonly IEmailConfirmRepository _emailConfirmRepository;

    public EmailConfirmService(IEmailConfirmRepository emailConfirmRepository)
    {
        _emailConfirmRepository = emailConfirmRepository;
    }

    public async Task CreateEmailConfirmAsync(Guid userId, string token)
    {
        await _emailConfirmRepository.CreateAsync(userId, token);
    }

    public async Task<Guid?> GetUserIdByTokenAsync(string token)
    {
        return await _emailConfirmRepository.GetUserIdByTokenAsync(token);
    }

    public async Task DeleteTokenByUserIdAsync(Guid userId)
    {
        await _emailConfirmRepository.DeleteByUserIdAsync(userId);
    }
}