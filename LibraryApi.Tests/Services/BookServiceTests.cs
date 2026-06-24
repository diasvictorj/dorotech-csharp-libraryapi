using FluentAssertions;
using LibraryApi.Application.DTOs;
using LibraryApi.Application.Exceptions;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;
using Moq;

namespace LibraryApi.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _bookService = new BookService(_bookRepositoryMock.Object, _authorRepositoryMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ShouldCreateBook_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Dom Casmurro",
            Isbn = "9788535914849",
            PublicationYear = 1899,
            AuthorId = 1
        };

        var author = new Author { Id = 1, Name = "Machado de Assis" };

        _bookRepositoryMock
            .Setup(r => r.GetByIsbnAsync(dto.Isbn))
            .ReturnsAsync((Book?)null);

        _authorRepositoryMock
            .Setup(r => r.GetByIdAsync(dto.AuthorId))
            .ReturnsAsync(author);

        _bookRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        _bookRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        _bookRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Book
            {
                Id = 1,
                Title = dto.Title,
                Isbn = dto.Isbn,
                PublicationYear = dto.PublicationYear,
                AuthorId = dto.AuthorId,
                Author = author
            });

        // Act
        var result = await _bookService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Isbn.Should().Be(dto.Isbn);
        result.AuthorName.Should().Be(author.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBusinessException_WhenIsbnAlreadyExists()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Dom Casmurro",
            Isbn = "9788535914849",
            PublicationYear = 1899,
            AuthorId = 1
        };

        _bookRepositoryMock
            .Setup(r => r.GetByIsbnAsync(dto.Isbn))
            .ReturnsAsync(new Book { Id = 1, Isbn = dto.Isbn });

        // Act
        var act = async () => await _bookService.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage($"*{dto.Isbn}*");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBusinessException_WhenAuthorNotFound()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Dom Casmurro",
            Isbn = "9788535914849",
            PublicationYear = 1899,
            AuthorId = 999
        };

        _bookRepositoryMock
            .Setup(r => r.GetByIsbnAsync(dto.Isbn))
            .ReturnsAsync((Book?)null);

        _authorRepositoryMock
            .Setup(r => r.GetByIdAsync(dto.AuthorId))
            .ReturnsAsync((Author?)null);

        // Act
        var act = async () => await _bookService.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage($"*{dto.AuthorId}*");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Dom Casmurro",
            Isbn = "9788535914849",
            PublicationYear = 1899,
            Author = new Author { Name = "Machado de Assis" }
        };

        _bookRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(book.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    // ── DeleteAsync ──────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Dom Casmurro" };

        _bookRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(book);

        _bookRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _bookService.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _bookRepositoryMock.Verify(r => r.Delete(book), Times.Once);
    }
}