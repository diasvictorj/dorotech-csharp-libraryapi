using System.ComponentModel.DataAnnotations;
using LibraryApi.Application.Validators;

namespace LibraryApi.Application.DTOs;

public class UpdateBookDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    [Isbn]
    public string Isbn { get; set; } = string.Empty;

    [Range(1000, 9999, ErrorMessage = "Ano de publicação inválido.")]
    public int PublicationYear { get; set; }

    [Required]
    public int AuthorId { get; set; }
}