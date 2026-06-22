using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.DTOs;

public class CreateBookDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    [MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1000, 9999, ErrorMessage = "Ano de publicação inválido.")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "O ID do autor é obrigatório.")]
    public int AuthorId { get; set; }
}