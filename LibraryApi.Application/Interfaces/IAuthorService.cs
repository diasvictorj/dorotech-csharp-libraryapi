using LibraryApi.Application.DTOs;

namespace LibraryApi.Application.Interfaces;

public interface IAuthorService
{
    Task<PagedResultDto<AuthorDto>> GetAllAsync(AuthorQueryDto query);
    Task<AuthorDto?> GetByIdAsync(int id);
    Task<AuthorDto> CreateAsync(CreateAuthorDto dto);
    Task<bool> UpdateAsync(int id, CreateAuthorDto dto);
    Task<bool> DeleteAsync(int id);
}