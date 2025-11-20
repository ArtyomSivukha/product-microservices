using Microsoft.Extensions.Options;

namespace UserManagement.Application.LinkURI;

public class LinkService : ILinkService
{
    private readonly Settings _settings;

    public LinkService(IOptions<Settings> emailSettings)
    {
        _settings = emailSettings.Value;
    }

    public Uri CreateLink(string token, string baseUri)
    {
        var uriBuilder = new UriBuilder(baseUri);
    
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["token"] = token;
    
        uriBuilder.Query = query.ToString();
        return uriBuilder.Uri;
    }
    
    public Uri CreateVerificationLink(string token)
    {
        return CreateLink(token, _settings.VerificationUri);
    }

    public Uri CreatePasswordResetLink(string token)
    {
        return CreateLink(token, _settings.PasswordResetUri);
    }
}