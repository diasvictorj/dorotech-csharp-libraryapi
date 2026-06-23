using LibraryApi.Application.DTOs;

namespace LibraryApi.Application.Interfaces;

public interface IBookService
{
    Task<PagedResultDto<BookDto>> GetAllAsync(BookQueryDto query);
    Task<BookDto?> GetByIdAsync(int id);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<bool> UpdateAsync(int id, UpdateBookDto dto);
    Task<bool> DeleteAsync(int id);
}