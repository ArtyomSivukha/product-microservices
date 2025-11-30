using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductManagement.Domain.Models;
using ProductManagement.Domain.Repositories;
using ProductManagement.Infrastructure.Database;
using ProductManagement.Infrastructure.Database.Entities;

namespace ProductManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductRepository(ProductDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        var products = await _dbContext.Products
            .Where(p => !p.IsHidden)
            .ToArrayAsync();
        return _mapper.Map<IEnumerable<ProductModel>>(products);
    }

    public async Task<IEnumerable<ProductModel>> GetAllByUserIdAsync(Guid userId)
    {
        var products = await _dbContext.Products
            .Where(p => p.UserId == userId && !p.IsHidden)
            .ToArrayAsync();
        return _mapper.Map<IEnumerable<ProductModel>>(products);
    }

    public async Task<ProductModel?> GetByIdAsync(Guid id)
    {
        var productEntity = await _dbContext.Products
            .Where(p =>!p.IsHidden)
            .FirstOrDefaultAsync(p => p.Id == id);
        return productEntity == null ? null : _mapper.Map<ProductModel>(productEntity);
    }

    public async Task<ProductModel?> GetByNameAsync(string name)
    {
        var productEntity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == name);
        return productEntity == null ? null : _mapper.Map<ProductModel>(productEntity);
    }

    public async Task<ProductModel> CreateProductAsync(ProductModel product)
    {
       var productEntity = _mapper.Map<Product>(product);
       _dbContext.Products.Add(productEntity);
       await _dbContext.SaveChangesAsync();
       return _mapper.Map<ProductModel>(productEntity);
    }

    public async Task UpdateProductAsync(ProductModel product)
    {
       var productEntity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
       if (productEntity is null)
       {
           throw new ArgumentNullException(nameof(productEntity), $"{nameof(productEntity)} is null");
       }
       productEntity.Name = product.Name;
       productEntity.Price = product.Price;
       productEntity.Description = product.Description;
       productEntity.UpdatedAt = DateTime.UtcNow;
       productEntity.IsAvailable = product.IsAvailable;
       await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var deleteProductEntity = await _dbContext.Products.FindAsync(id);
        if (deleteProductEntity is null)
        {
            throw new ArgumentNullException(nameof(deleteProductEntity), $"{nameof(deleteProductEntity)} is null");
        }

        _dbContext.Products.Remove(deleteProductEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductModel>> SearchAsync(
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId)
    {
        var query = _dbContext.Products.AsQueryable();

        query = query.Where(p => !p.IsHidden);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var lowered = name.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowered));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(p => p.IsAvailable == isAvailable.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        var products = await query.ToArrayAsync();
        return _mapper.Map<IEnumerable<ProductModel>>(products);
    }

    public async Task HideByUserIdAsync(Guid userId)
    {
        var products = await _dbContext.Products
            .Where(p => p.UserId == userId && !p.IsHidden)
            .ToListAsync();

        if (!products.Any())
            return;

        foreach (var product in products)
        {
            product.IsHidden = true;
            product.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task ShowByUserIdAsync(Guid userId)
    {
        var products = await _dbContext.Products
            .Where(p => p.UserId == userId && p.IsHidden)
            .ToListAsync();

        if (!products.Any())
            return;

        foreach (var product in products)
        {
            product.IsHidden = false;
            product.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}