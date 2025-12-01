using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(UserDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var users = await _dbContext.Users.ToArrayAsync();
        return _mapper.Map<IEnumerable<UserModel>>(users);
    }

    public async Task<UserModel?> GetByIdAsync(Guid id)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return userEntity is null ? null : _mapper.Map<UserModel>(userEntity);
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        return userEntity is null ? null : _mapper.Map<UserModel>(userEntity);
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        return userEntity is null ? null : _mapper.Map<UserModel>(userEntity);
    }
    
    public async Task<UserModel> CreateAsync(UserModel user)
    {
        var userEntity = _mapper.Map<User>(user);
        _dbContext.Users.Add(userEntity);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<UserModel>(userEntity);
    }

    public async Task UpdateAsync(UserModel user)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        if (userEntity is null)
        {
            throw new ArgumentNullException(nameof(userEntity), $"{nameof(userEntity)} is null");
        }
        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        userEntity.Password = user.Password;
        userEntity.IsEmailConfirmed = user.IsEmailConfirmed;
        userEntity.IsActive = user.IsActive;
        userEntity.Modified = DateTime.UtcNow;
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