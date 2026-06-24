using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _context;

    public AuthorRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Author> Authors, int TotalCount)> GetAllAsync(
        string? name,
        int page,
        int pageSize)
    {
        var queryable = _context.Authors
            .Include(a => a.Books)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            queryable = queryable.Where(a => a.Name.Contains(name.Trim()));

        var totalCount = await queryable.CountAsync();

        var authors = await queryable
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (authors, totalCount);
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Author?> GetByNameAsync(string name)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
    }

    public async Task AddAsync(Author author)
    {
        await _context.Authors.AddAsync(author);
    }

    public void Update(Author author)
    {
        _context.Authors.Update(author);
    }

    public void Delete(Author author)
    {
        _context.Authors.Remove(author);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}