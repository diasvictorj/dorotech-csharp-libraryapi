using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}