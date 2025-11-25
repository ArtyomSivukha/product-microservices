namespace UserManagement.Application.Products;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ProductService");
    }

    public async Task HideProductsByUserAsync(Guid userId, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(
              HttpMethod.Post, 
            $"/api/products/hide/by-user/{userId}");

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task ShowProductsByUserAsync(Guid userId, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post, 
            $"/api/products/show/by-user/{userId}");

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}