using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.Validators;

public class IsbnConsistentAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dto = validationContext.ObjectInstance as DTOs.CreateBookDto;
        if (dto is null)
            return ValidationResult.Success;

        if (dto.GenerateIsbn && !string.IsNullOrWhiteSpace(dto.Isbn))
            return new ValidationResult(
                "Não é possível informar um ISBN quando GenerateIsbn é true. " +
                "Remova o ISBN ou defina GenerateIsbn como false.");

        return ValidationResult.Success;
    }
}