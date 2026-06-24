using LibraryApi.Application.DTOs;
using LibraryApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.API.Controllers;

/// <summary>
/// Gerenciamento de autores da livraria.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary>
    /// Retorna todos os autores cadastrados com suporte a filtros e paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<AuthorDto>>> GetAll([FromQuery] AuthorQueryDto query)
    {
        var result = await _authorService.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Retorna um autor pelo seu identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> GetById(int id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author is null)
            return NotFound(new { message = $"Autor com id {id} não encontrado." });

        return Ok(author);
    }

    /// <summary>
    /// Cadastra um novo autor. Requer autenticação de Administrador.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorDto dto)
    {
        var created = await _authorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um autor existente. Requer autenticação de Administrador.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, CreateAuthorDto dto)
    {
        var updated = await _authorService.UpdateAsync(id, dto);
        if (!updated)
            return NotFound(new { message = $"Autor com id {id} não encontrado." });

        return NoContent();
    }

    /// <summary>
    /// Remove um autor. Requer autenticação de Administrador.
    /// Não é possível remover autores que possuem livros cadastrados.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _authorService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Autor com id {id} não encontrado." });

        return NoContent();
    }
}