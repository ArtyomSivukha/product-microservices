namespace UserManagement.Application.Products;

public interface IProductServiceClient
{
    Task HideProductsByUserAsync(Guid userId, string? bearerToken);
    Task ShowProductsByUserAsync(Guid userId, string? bearerToken);
}