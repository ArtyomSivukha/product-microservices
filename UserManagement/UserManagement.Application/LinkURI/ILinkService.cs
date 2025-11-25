namespace UserManagement.Application.LinkURI;

public interface ILinkService
{
    Uri CreateVerificationLink(string token);
    Uri CreatePasswordResetLink(string token);
}