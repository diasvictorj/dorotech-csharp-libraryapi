using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task AddAsync(Book book);
    void Update(Book book);
    void Delete(Book book);
    Task<bool> SaveChangesAsync();
}