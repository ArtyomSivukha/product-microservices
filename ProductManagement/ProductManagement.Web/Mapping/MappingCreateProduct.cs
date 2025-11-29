using AutoMapper;
using ProductManagement.Domain.Models;
using ProductManagement.Web.ModelsDTO;

namespace ProductManagement.Web.Mapping;

public class MappingCreateProduct : Profile
{
    public MappingCreateProduct()
    {
        CreateMap<CreateProductRequest, ProductModel>();
    }
}