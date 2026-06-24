using LibraryApi.Application.DTOs;
using LibraryApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domain.Enums;

namespace LibraryApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
        {
            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }

        return Ok(result);
    }



    /// <summary>
    /// Registra um novo usuário.
    /// Criar usuários Admin requer autenticação de Administrador.
    /// Usuários Públicos podem ser criados sem autenticação.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        // Apenas Admin pode criar outro Admin
        if (dto.Role == UserRole.Admin && !User.IsInRole("Admin"))
            return Forbid();

        var result = await _authService.RegisterAsync(dto);
        return CreatedAtAction(nameof(Login), result);
    }

    /// <summary>
    /// Renova o JWT usando um Refresh Token válido.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenDto dto)
    {
        var result = await _authService.RefreshAsync(dto);
        if (result is null)
            return Unauthorized(new { message = "Refresh token inválido ou expirado." });

        return Ok(result);
    }
}

