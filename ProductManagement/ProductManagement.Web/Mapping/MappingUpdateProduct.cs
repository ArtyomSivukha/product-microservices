using AutoMapper;
using ProductManagement.Domain.Models;
using ProductManagement.Web.ModelsDTO;

namespace ProductManagement.Web.Mapping;

public class MappingUpdateProduct :Profile
{
    public MappingUpdateProduct()
    {
        CreateMap<UpdateProductRequest, ProductModel>();
    }
}