using LibraryApi.Application.DTOs;
using LibraryApi.Application.Exceptions;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;

namespace LibraryApi.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    public async Task<PagedResultDto<BookDto>> GetAllAsync(BookQueryDto query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1 || query.PageSize > 50) query.PageSize = 10;

        var (books, totalCount) = await _bookRepository.GetAllAsync(
         query.Title,
         query.AuthorName,
         query.PublicationYear,
         query.Page,
         query.PageSize);

        return new PagedResultDto<BookDto>
        {
            Data = books.Select(MapToDto),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var existing = await _bookRepository.GetByIsbnAsync(dto.Isbn);
        if (existing is not null)
            throw new BusinessException($"Já existe um livro cadastrado com o ISBN '{dto.Isbn}'.");

        // Correção do bug: valida se o autor existe
        var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
        if (author is null)
            throw new BusinessException($"Autor com id {dto.AuthorId} não encontrado.");

        var book = new Book
        {
            Title = dto.Title,
            Isbn = dto.Isbn,
            PublicationYear = dto.PublicationYear,
            AuthorId = dto.AuthorId
        };

        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        var created = await _bookRepository.GetByIdAsync(book.Id);
        return MapToDto(created!);
    }



    public async Task<BookDto?> GetByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? null : MapToDto(book);
    }
    public async Task<bool> UpdateAsync(int id, UpdateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return false;
        }

        var existing = await _bookRepository.GetByIsbnAsync(dto.Isbn);
        if (existing is not null && existing.Id != id)
        {
            throw new BusinessException($"Já existe outro livro cadastrado com o ISBN '{dto.Isbn}'.");
        }

        book.Title = dto.Title;
        book.Isbn = dto.Isbn;
        book.PublicationYear = dto.PublicationYear;
        book.AuthorId = dto.AuthorId;

        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return false;
        }

        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();

        return true;
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationYear = book.PublicationYear,
            AuthorName = book.Author?.Name ?? string.Empty
        };
    }
}