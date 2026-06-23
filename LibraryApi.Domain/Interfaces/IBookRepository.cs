using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IBookRepository
{
    Task<(IEnumerable<Book> Books, int TotalCount)> GetAllAsync(
        string? title,
        string? authorName,
        int? publicationYear,
        int page,
        int pageSize);
    Task<Book?> GetByIdAsync(int id);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task AddAsync(Book book);
    void Update(Book book);
    void Delete(Book book);
    Task<bool> SaveChangesAsync();
}