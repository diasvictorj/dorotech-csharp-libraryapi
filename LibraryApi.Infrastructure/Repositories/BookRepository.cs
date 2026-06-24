using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Book> Books, int TotalCount)> GetAllAsync(
       string? title,
       string? authorName,
       int? publicationYear,
       string? orderBy,
       string? orderDirection,
       int page,
       int pageSize)
    {
        var queryable = _context.Books
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            queryable = queryable.Where(b => b.Title.Contains(title.Trim()));

        if (!string.IsNullOrWhiteSpace(authorName))
            queryable = queryable.Where(b => b.Author.Name.Contains(authorName.Trim()));

        if (publicationYear.HasValue)
            queryable = queryable.Where(b => b.PublicationYear == publicationYear);

        var totalCount = await queryable.CountAsync();

        var isDesc = orderDirection?.ToLower() == "desc";

        queryable = orderBy?.ToLower() switch
        {
            "author" => isDesc ? queryable.OrderByDescending(b => b.Author.Name)
                                        : queryable.OrderBy(b => b.Author.Name),
            "publicationyear" => isDesc ? queryable.OrderByDescending(b => b.PublicationYear)
                                        : queryable.OrderBy(b => b.PublicationYear),
            "isbn" => isDesc ? queryable.OrderByDescending(b => b.Isbn)
                                        : queryable.OrderBy(b => b.Isbn),
            _ => isDesc ? queryable.OrderByDescending(b => b.Title)
                                        : queryable.OrderBy(b => b.Title)
        };

        var books = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (books, totalCount);
    }
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
    }

    public void Update(Book book)
    {
        _context.Books.Update(book);
    }

    public void Delete(Book book)
    {
        _context.Books.Remove(book);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}