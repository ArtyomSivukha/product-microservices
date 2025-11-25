namespace UserManagement.Application;

public interface ICurrentUserAccessor
{
    public Guid UserId { get; }
    public string UserToken { get; }
}