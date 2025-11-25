using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Repositories;

public class EmailConfirmRepository : IEmailConfirmRepository
{
    private readonly UserDbContext _context;
    
    public EmailConfirmRepository(UserDbContext userDbContext)
    {
        _context = userDbContext;
    }

    public async Task CreateAsync(Guid userId, string token)
    {
        var emailToken = new EmailConfirm
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token
        };
        
        _context.EmailConfirmationTokens.Add(emailToken);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid?> GetUserIdByTokenAsync(string token)
    {
        var emailToken = await _context.EmailConfirmationTokens
            .FirstOrDefaultAsync(x => x.Token == token);
            
        return emailToken?.UserId;
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var existingToken = await _context.EmailConfirmationTokens
            .FirstOrDefaultAsync(x => x.UserId == userId);
            
        if (existingToken is not null)
        {
            _context.EmailConfirmationTokens.Remove(existingToken);
            await _context.SaveChangesAsync();
        }
    }
}