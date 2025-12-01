namespace ProductManagement.Application.Users;

public interface IUserServiceClient
{
    Task<UserInfoDto?> GetUserByIdAsync(Guid id, string? bearerToken = null);
}