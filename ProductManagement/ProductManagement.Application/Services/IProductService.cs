using ProductManagement.Domain.Models;

namespace ProductManagement.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductModel>> GetAllAsync();

    Task<IEnumerable<ProductModel>> GetByUserIdAsync(Guid userId);
    Task<ProductModel?> GetByIdAsync(Guid id);
    Task<ProductModel?> GetByNameAsync(string username);
    Task<ProductModel> CreateProductAsync(ProductModel productModel);
    Task UpdateAsync(ProductModel productModel);
    Task DeleteAsync(Guid productId, Guid currentUserId);
    Task<IEnumerable<ProductModel>> SearchAsync(
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId);
    Task HideByUserIdAsync(Guid userId);
    Task ShowByUserIdAsync(Guid userId);
}