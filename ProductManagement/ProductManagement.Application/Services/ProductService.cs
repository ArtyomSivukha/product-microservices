using Microsoft.AspNetCore.Http;
using ProductManagement.Application.Users;
using ProductManagement.Domain.Models;
using ProductManagement.Domain.Repositories;

namespace ProductManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly UserServiceClient _userServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductService(
        UserServiceClient userServiceClient,
        IProductRepository productRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userServiceClient = userServiceClient;
        _productRepository = productRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<IEnumerable<ProductModel>> GetByUserIdAsync(Guid userId)
    {
        return await _productRepository.GetAllByUserIdAsync(userId);
    }

    public async Task<ProductModel?> GetByIdAsync(Guid id)
    {
        var products = await _productRepository.GetByIdAsync(id);
        if (products is null)
        {
            throw new ArgumentNullException(nameof(products), $"{nameof(products)} is null");
        }

        return products;
    }

    public Task<ProductModel?> GetByNameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductModel> CreateProductAsync(ProductModel productModel)
    {
        await EnsureActiveUserAsync(productModel.UserId);

        productModel.IsHidden = false;
        productModel.IsAvailable = true;

        return await _productRepository.CreateProductAsync(productModel);
    }

    public async Task UpdateAsync(ProductModel productModel)
    {
        await EnsureActiveUserAsync(productModel.UserId);
        await _productRepository.UpdateProductAsync(productModel);
    }

    public async Task DeleteAsync(Guid productId, Guid currentUserId)
    {
        await EnsureActiveUserAsync(currentUserId);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product), $"{nameof(product)} is null");
        }

        EnsureOwner(product, currentUserId);

        await _productRepository.DeleteProductAsync(productId);
    }

    public async Task<IEnumerable<ProductModel>> SearchAsync(
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId)
    {
        return await _productRepository.SearchAsync(name, minPrice, maxPrice, isAvailable, userId);
    }

    public async Task HideByUserIdAsync(Guid userId)
    {
        await _productRepository.HideByUserIdAsync(userId);
    }

    public async Task ShowByUserIdAsync(Guid userId)
    {
        await _productRepository.ShowByUserIdAsync(userId);
    }

    private string? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<UserInfoDto> EnsureActiveUserAsync(Guid userId)
    {
        var token = GetAccessToken();
        var user = await _userServiceClient.GetUserByIdAsync(userId, token);

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        if (!user.IsEmailConfirmed)
            throw new UnauthorizedAccessException("Email is not confirmed");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is deactivated");

        return user;
    }

    private static void EnsureOwner(ProductModel productModel, Guid userId)
    {
        if (productModel.UserId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this product");
    }
}