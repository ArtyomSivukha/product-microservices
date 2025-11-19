using Microsoft.Extensions.Options;

namespace UserManagement.Application.LinkURI;

public class LinkService : ILinkService
{
    private readonly EmailSettings _emailSettings;

    public LinkService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public Uri CreateVerificationLink(string token)
    {
        var baseUri = _emailSettings.VerificationUri;
        var uriBuilder = new UriBuilder(baseUri);
        
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["token"] = token;
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.Uri;
    }
}