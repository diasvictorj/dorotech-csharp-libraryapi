using LibraryApi.Application.DTOs;

namespace LibraryApi.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> RefreshAsync(RefreshTokenDto dto);
}