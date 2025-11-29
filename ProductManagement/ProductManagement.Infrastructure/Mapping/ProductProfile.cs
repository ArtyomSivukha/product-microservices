using AutoMapper;
using ProductManagement.Domain.Models;
using ProductManagement.Infrastructure.Database.Entities;

namespace ProductManagement.Infrastructure.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductModel>();
        CreateMap<ProductModel, Product>();
    }
}