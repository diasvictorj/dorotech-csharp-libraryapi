using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LibraryApi.Application.Validators;

public class IsbnAttribute : ValidationAttribute
{
    public IsbnAttribute() : base("ISBN inválido. Informe um ISBN-10 ou ISBN-13 válido.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success; // deixa o [Required] cuidar disso

        var isbn = value.ToString()!.Replace("-", "").Replace(" ", "");

        if (isbn.Length == 10 && IsValidIsbn10(isbn))
            return ValidationResult.Success;

        if (isbn.Length == 13 && IsValidIsbn13(isbn))
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }

    private static bool IsValidIsbn10(string isbn)
    {
        if (!Regex.IsMatch(isbn, @"^\d{9}[\dX]$"))
            return false;

        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (isbn[i] - '0') * (10 - i);

        var last = isbn[9] == 'X' ? 10 : isbn[9] - '0';
        sum += last;

        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string isbn)
    {
        if (!Regex.IsMatch(isbn, @"^\d{13}$"))
            return false;

        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);

        var checkDigit = (10 - sum % 10) % 10;
        return checkDigit == isbn[12] - '0';
    }
}