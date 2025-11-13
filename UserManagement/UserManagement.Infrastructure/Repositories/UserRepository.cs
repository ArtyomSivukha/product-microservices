using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User?>> GetAllAsync()
    {
        return await _dbContext.Users.ToArrayAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> CreateAsync(User? user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }

        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleteUserEntity = await _dbContext.Users.FindAsync(id);
        if (deleteUserEntity is null)
        {
            throw new ArgumentNullException(nameof(deleteUserEntity), $"{nameof(deleteUserEntity)} is null");
        }

        _dbContext.Users.Remove(deleteUserEntity);
        await _dbContext.SaveChangesAsync();
    }
}