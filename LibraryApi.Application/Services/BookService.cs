using LibraryApi.Application.DTOs;
using LibraryApi.Application.Exceptions;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;

namespace LibraryApi.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? null : MapToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var existing = await _bookRepository.GetByIsbnAsync(dto.Isbn);
        if (existing is not null)
        {
            throw new BusinessException($"Já existe um livro cadastrado com o ISBN '{dto.Isbn}'.");
        }

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