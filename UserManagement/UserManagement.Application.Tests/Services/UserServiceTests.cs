using FluentAssertions;
using Moq;
using UserManagement.Application.LinkURI;
using UserManagement.Application.Products;
using UserManagement.Application.Services.EmailConfirmService;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Users;

namespace UserManagement.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<ILinkService> _linkServiceMock = new();
    private readonly Mock<IEmailConfirmService> _emailConfirmServiceMock = new();
    private readonly Mock<IProductServiceClient> _productServiceClientMock = new();
    private readonly Mock<ICurrentUserAccessor> _currentUserAccessorMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _currentUserAccessorMock.SetupGet(a => a.UserToken).Returns("token-value");
        _sut = new UserService(
            _userRepositoryMock.Object,
            _emailSenderMock.Object,
            _linkServiceMock.Object,
            _emailConfirmServiceMock.Object,
            _productServiceClientMock.Object,
            _currentUserAccessorMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldHashPasswordAndSendConfirmation()
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Username = "john",
            Email = "john@example.com",
            Password = "plain"
        };

        var createdUser = new UserModel
        {
            Id = user.Id,
            Email = user.Email
        };

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<UserModel>()))
            .ReturnsAsync((UserModel model) =>
            {
                model.Id = createdUser.Id;
                return model;
            });

        var capturedToken = string.Empty;
        _emailConfirmServiceMock
            .Setup(s => s.CreateEmailConfirmAsync(createdUser.Id, It.IsAny<string>()))
            .Callback<Guid, string>((_, token) => capturedToken = token)
            .Returns(Task.CompletedTask);

        _linkServiceMock
            .Setup(l => l.CreateVerificationLink(It.IsAny<string>()))
            .Returns(new Uri("https://example.com/verify"));
        
        var result = await _sut.RegisterUserAsync(user, "plain");
        
        result.Id.Should().Be(createdUser.Id);
        result.Email.Should().Be(createdUser.Email);
        result.Password.Should().NotBeNullOrWhiteSpace();
        user.IsEmailConfirmed.Should().BeFalse();
        user.IsActive.Should().BeFalse();

        _emailConfirmServiceMock.Verify(s => s.CreateEmailConfirmAsync(createdUser.Id, capturedToken), Times.Once);
        _linkServiceMock.Verify(l => l.CreateVerificationLink(capturedToken), Times.Once);
        _emailSenderMock.Verify(s => s.SendEmailAsync(It.Is<Message>(m => m.To.Contains(user.Email))), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrow_WhenPasswordsDoNotMatch()
    {
        var user = new UserModel
        {
            Username = "john",
            Email = "john@example.com",
            Password = "pass1"
        };
        
        var act = async () => await _sut.RegisterUserAsync(user, "pass2");
        
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Passwords do not match");
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserModel>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldDeactivateAndHideProducts()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserModel>()))
            .Returns(Task.CompletedTask);
        
        await _sut.DeactivateUserAsync(userId);
        
        user.IsActive.Should().BeFalse();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _productServiceClientMock.Verify(c => c.HideProductsByUserAsync(userId, "token-value"), Times.Once);
    }
}

