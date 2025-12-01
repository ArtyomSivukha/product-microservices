using System.Net;
using System.Net.Http.Json;

namespace ProductManagement.Application.Users;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UserService");
    }

    public async Task<UserInfoDto?> GetUserByIdAsync(Guid id, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{id}");

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserInfoDto>();
        return user;
    }
}