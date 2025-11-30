using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Services;
using ProductManagement.Domain.Models;
using ProductManagement.Web.ModelsDTO;
using AccessViolationException = ProductManagement.Application.Exceptions.AccessViolationException;

namespace ProductManagement.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var model = _mapper.Map<ProductModel>(request);
        model.UserId = userId;
        var created = await _productService.CreateProductAsync(model);
        return Ok(created);
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> GetMyProducts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var products = await _productService.GetByUserIdAsync(userId);
        return Ok(products);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existing = await _productService.GetByIdAsync(id);
        if (existing is null)
            throw new ArgumentNullException(nameof(id), "Product not found");

        if (existing.UserId != userId)
            throw new AccessViolationException("This product can be edited only by owner");

        var model = _mapper.Map<ProductModel>(request);

        model.Id = id;
        model.UserId = userId;
        model.Name = request.Name ?? model.Name;
        model.Description = request.Description ?? model.Description;
        model.Price = request.Price ?? model.Price;
            
        await _productService.UpdateAsync(model);
        
        return Ok(model);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _productService.DeleteAsync(id, userId);

        return Ok();
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
    {
        var products = await _productService.SearchAsync(
            request.Name,
            request.MinPrice,
            request.MaxPrice,
            request.IsAvailable,
            request.UserId);
        return Ok(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    [HttpPost("hide/by-user/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HideByUser(Guid userId)
    {
        await _productService.HideByUserIdAsync(userId);
        return Ok();
    }
    
    [HttpPost("show/by-user/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ShowByUser(Guid userId)
    {
        await _productService.ShowByUserIdAsync(userId);
        return Ok();
    }
}