using LibraryApi.Application.DTOs;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Application.Exceptions;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user is null)
        {
            return null;
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return null;
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
{
    var existing = await _userRepository.GetByUsernameAsync(dto.Username);
    if (existing is not null)
        throw new BusinessException($"Já existe um usuário com o username '{dto.Username}'.");

    var user = new User
    {
        Username = dto.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        Role = dto.Role
    };

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    var token = _tokenService.GenerateToken(user);

    return new AuthResponseDto
    {
        Token = token,
        Username = user.Username,
        Role = user.Role.ToString()
    };
}
}