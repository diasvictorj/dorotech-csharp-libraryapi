using FluentAssertions;
using LibraryApi.Application.DTOs;
using LibraryApi.Application.Interfaces;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Enums;
using LibraryApi.Domain.Interfaces;
using Moq;

namespace LibraryApi.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(new LoginDto
        {
            Username = "nonexistent",
            Password = "any"
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsWrong()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password"),
            Role = UserRole.Admin
        };

        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(new LoginDto
        {
            Username = "admin",
            Password = "wrong_password"
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin
        };

        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns("fake_jwt_token");

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns("fake_refresh_token");

        _refreshTokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _authService.LoginAsync(new LoginDto
        {
            Username = "admin",
            Password = "Admin@123"
        });

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be("fake_jwt_token");
        result.Username.Should().Be("admin");
        result.Role.Should().Be("Admin");
    }
}