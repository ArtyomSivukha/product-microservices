namespace UserManagement.Application.LinkURI;

public interface ILinkService
{
    public Uri CreateLink(string token, string baseUri);
    Uri CreateVerificationLink(string token);
    Uri CreatePasswordResetLink(string token);
}