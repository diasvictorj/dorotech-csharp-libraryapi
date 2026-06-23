using LibraryApi.Application.DTOs;
using LibraryApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.API.Controllers;

/// <summary>
/// Gerenciamento de livros da livraria.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retorna todos os livros cadastrados com suporte a filtros e paginação.
    /// </summary>
    /// <param name="query">Parâmetros de filtro e paginação.</param>
    /// <returns>Lista paginada de livros ordenados pelo título.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<BookDto>>> GetAll([FromQuery] BookQueryDto query)
    {
        var result = await _bookService.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Retorna um livro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do livro.</param>
    /// <returns>Dados do livro encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null)
            return NotFound(new { message = $"Livro com id {id} não encontrado." });

        return Ok(book);
    }

    /// <summary>
    /// Cadastra um novo livro. Requer autenticação de Administrador.
    /// </summary>
    /// <param name="dto">Dados do livro a ser cadastrado.</param>
    /// <returns>Livro cadastrado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookDto>> Create(CreateBookDto dto)
    {
        var created = await _bookService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um livro existente. Requer autenticação de Administrador.
    /// </summary>
    /// <param name="id">Identificador do livro.</param>
    /// <param name="dto">Dados atualizados do livro.</param>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, UpdateBookDto dto)
    {
        var updated = await _bookService.UpdateAsync(id, dto);
        if (!updated)
            return NotFound(new { message = $"Livro com id {id} não encontrado." });

        return NoContent();
    }

    /// <summary>
    /// Remove um livro. Requer autenticação de Administrador.
    /// </summary>
    /// <param name="id">Identificador do livro.</param>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Livro com id {id} não encontrado." });

        return NoContent();
    }
}