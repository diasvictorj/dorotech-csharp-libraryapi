using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<(IEnumerable<Author> Authors, int TotalCount)> GetAllAsync(
      string? name,
      string? orderBy,
      string? orderDirection,
      int page,
      int pageSize);
    Task<Author?> GetByIdAsync(int id);
    Task<Author?> GetByNameAsync(string name);
    Task AddAsync(Author author);
    void Update(Author author);
    void Delete(Author author);
    Task<bool> SaveChangesAsync();
}