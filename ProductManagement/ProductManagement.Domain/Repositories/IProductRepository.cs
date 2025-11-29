using ProductManagement.Domain.Models;

namespace ProductManagement.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductModel>> GetAllAsync();
    Task<IEnumerable<ProductModel>> GetAllByUserIdAsync(Guid userId);
    Task<ProductModel?> GetByIdAsync(Guid id);
    Task<ProductModel?> GetByNameAsync(string name);
    Task<ProductModel> CreateProductAsync(ProductModel product);
    Task UpdateProductAsync(ProductModel product);
    Task DeleteProductAsync(Guid id);
    Task<IEnumerable<ProductModel>> SearchAsync(
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId);
    
    Task HideByUserIdAsync(Guid userId);
    Task ShowByUserIdAsync(Guid userId);
}