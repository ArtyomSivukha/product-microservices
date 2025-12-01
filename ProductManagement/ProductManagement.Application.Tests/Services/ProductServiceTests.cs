using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ProductManagement.Application.Services;
using ProductManagement.Application.Users;
using ProductManagement.Domain.Models;
using ProductManagement.Domain.Repositories;

namespace ProductManagement.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly Mock<IUserServiceClient> _userServiceClientMock = new();
    private readonly DefaultHttpContext _httpContext = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(_httpContext);

        _sut = new ProductService(
            _userServiceClientMock.Object,
            _repositoryMock.Object,
            httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldSetFlagsAndCallRepository()
    {
        var userId = Guid.NewGuid();
        _httpContext.Request.Headers["Authorization"] = "Bearer token123";
        _userServiceClientMock
            .Setup(c => c.GetUserByIdAsync(userId, "token123"))
            .ReturnsAsync(new UserInfoDto
            {
                Id = userId,
                IsActive = true,
                IsEmailConfirmed = true
            });

        _repositoryMock
            .Setup(r => r.CreateProductAsync(It.IsAny<ProductModel>()))
            .ReturnsAsync((ProductModel p) => p);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Description = "Desc",
            UserId = userId,
            Price = 10
        };

        var created = await _sut.CreateProductAsync(product);

        created.IsAvailable.Should().BeTrue();
        created.IsHidden.Should().BeFalse();
        _repositoryMock.Verify(r => r.CreateProductAsync(product), Times.Once);
    }

    [Theory]
    [InlineData(false, true, "Email is not confirmed")]
    [InlineData(true, false, "User is deactivated")]
    public async Task CreateProductAsync_ShouldThrow_WhenUserInvalid(bool isEmailConfirmed, bool isActive, string expectedMessage)
    {
        var userId = Guid.NewGuid();
        _userServiceClientMock
            .Setup(c => c.GetUserByIdAsync(userId, It.IsAny<string>()))
            .ReturnsAsync(new UserInfoDto
            {
                Id = userId,
                IsEmailConfirmed = isEmailConfirmed,
                IsActive = isActive
            });

        var product = new ProductModel
        {
            UserId = userId
        };

        var act = async () => await _sut.CreateProductAsync(product);

        var exception = await act.Should().ThrowAsync<UnauthorizedAccessException>();
        exception.Which.Message.Should().Be(expectedMessage);
        _repositoryMock.Verify(r => r.CreateProductAsync(It.IsAny<ProductModel>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldValidateOwnershipBeforeDeleting()
    {
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new ProductModel { UserId = ownerId };

        _userServiceClientMock
            .Setup(c => c.GetUserByIdAsync(ownerId, It.IsAny<string>()))
            .ReturnsAsync(new UserInfoDto
            {
                Id = ownerId,
                IsActive = true,
                IsEmailConfirmed = true
            });

        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        await _sut.DeleteAsync(productId, ownerId);

        _repositoryMock.Verify(r => r.DeleteProductAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenProductDoesNotBelongToUser()
    {
        var ownerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var product = new ProductModel { UserId = ownerId };

        _userServiceClientMock
            .Setup(c => c.GetUserByIdAsync(differentUserId, It.IsAny<string>()))
            .ReturnsAsync(new UserInfoDto
            {
                Id = differentUserId,
                IsActive = true,
                IsEmailConfirmed = true
            });

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        var act = async () => await _sut.DeleteAsync(Guid.NewGuid(), differentUserId);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _repositoryMock.Verify(r => r.DeleteProductAsync(It.IsAny<Guid>()), Times.Never);
    }
}

