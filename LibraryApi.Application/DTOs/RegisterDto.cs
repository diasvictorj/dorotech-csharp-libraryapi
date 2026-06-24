using System.ComponentModel.DataAnnotations;
using LibraryApi.Domain.Enums;

namespace LibraryApi.Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "O username é obrigatório.")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Public;
}