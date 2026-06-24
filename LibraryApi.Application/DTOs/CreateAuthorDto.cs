using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.DTOs;

public class CreateAuthorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}