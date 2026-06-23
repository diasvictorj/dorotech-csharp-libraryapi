using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int id);
    Task AddAsync(Author author);
    Task<bool> SaveChangesAsync();
}