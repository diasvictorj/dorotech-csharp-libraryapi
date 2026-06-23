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

    public async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _context.Authors
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors.FindAsync(id);
    }

    public async Task AddAsync(Author author)
    {
        await _context.Authors.AddAsync(author);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}