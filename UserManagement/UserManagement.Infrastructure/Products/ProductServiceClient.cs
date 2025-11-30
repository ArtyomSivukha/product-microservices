using UserManagement.Application.Exceptions;
using UserManagement.Application.Products;

namespace UserManagement.Infrastructure.Products;

public class ProductServiceClient : IProductServiceClient
{
    private const string ClientName = "ProductService";
    private readonly IHttpClientFactory _clientFactory;
    
    public ProductServiceClient(IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
    }

    public async Task HideProductsByUserAsync(Guid userId, string? bearerToken)
    {
        var request = new HttpRequestMessage(
              HttpMethod.Post, 
            $"/api/products/hide/by-user/{userId}");

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }
        
        using var client = _clientFactory.CreateClient(ClientName);

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new ExternalServiceAvailabilityException();
        }
        
    }
    
    public async Task ShowProductsByUserAsync(Guid userId, string? bearerToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post, 
            $"/api/products/show/by-user/{userId}");

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        using var client = _clientFactory.CreateClient(ClientName);
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new ExternalServiceAvailabilityException();
        }
        
    }
}