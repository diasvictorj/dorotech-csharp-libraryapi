using LibraryApi.Application.DTOs;
using LibraryApi.Application.Exceptions;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces;

namespace LibraryApi.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<PagedResultDto<AuthorDto>> GetAllAsync(AuthorQueryDto query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1 || query.PageSize > 50) query.PageSize = 10;

        var (authors, totalCount) = await _authorRepository.GetAllAsync(
    query.Name,
    query.OrderBy,
    query.OrderDirection,
    query.Page,
    query.PageSize);

        return new PagedResultDto<AuthorDto>
        {
            Data = authors.Select(MapToDto),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AuthorDto?> GetByIdAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        return author is null ? null : MapToDto(author);
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto)
    {
        var existing = await _authorRepository.GetByNameAsync(dto.Name);
        if (existing is not null)
            throw new BusinessException($"Já existe um autor cadastrado com o nome '{dto.Name}'.");

        var author = new Author { Name = dto.Name };

        await _authorRepository.AddAsync(author);
        await _authorRepository.SaveChangesAsync();

        var created = await _authorRepository.GetByIdAsync(author.Id);
        return MapToDto(created!);
    }

    public async Task<bool> UpdateAsync(int id, CreateAuthorDto dto)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author is null)
            return false;

        var existing = await _authorRepository.GetByNameAsync(dto.Name);
        if (existing is not null && existing.Id != id)
            throw new BusinessException($"Já existe outro autor cadastrado com o nome '{dto.Name}'.");

        author.Name = dto.Name;

        _authorRepository.Update(author);
        await _authorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author is null)
            return false;

        if (author.Books.Any())
            throw new BusinessException($"Não é possível excluir o autor '{author.Name}' pois ele possui livros cadastrados.");

        _authorRepository.Delete(author);
        await _authorRepository.SaveChangesAsync();

        return true;
    }

    private static AuthorDto MapToDto(Author author) => new()
    {
        Id = author.Id,
        Name = author.Name,
        BookCount = author.Books?.Count ?? 0
    };
}