using System.ComponentModel.DataAnnotations;
using LibraryApi.Application.Validators;

namespace LibraryApi.Application.DTOs;

[IsbnConsistent]
public class CreateBookDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// ISBN-10 ou ISBN-13 do livro. Ignorado se GenerateIsbn for true.
    /// </summary>
    [MaxLength(20)]
    public string? Isbn { get; set; }

    [Range(1000, 9999, ErrorMessage = "Ano de publicação inválido.")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "O ID do autor é obrigatório.")]
    public int AuthorId { get; set; }

    /// <summary>
    /// Se verdadeiro, um ISBN-13 válido será gerado automaticamente.
    /// Não informe o campo Isbn quando esta flag estiver ativa.
    /// </summary>
    public bool GenerateIsbn { get; set; } = false;
}