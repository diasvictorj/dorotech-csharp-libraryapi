using FluentAssertions;
using LibraryApi.Application.Validators;

namespace LibraryApi.Tests.Validators;

public class IsbnValidatorTests
{
    private readonly IsbnAttribute _validator = new();

    [Theory]
    [InlineData("9788535914849")]  // ISBN-13 válido
    [InlineData("9788532511010")]  // ISBN-13 válido
    [InlineData("0306406152")]     // ISBN-10 válido
    public void IsValid_ShouldReturnTrue_WhenIsbnIsValid(string isbn)
    {
        var result = _validator.IsValid(isbn);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("123")]            // muito curto
    [InlineData("1234567890000")] // 13 dígitos mas dígito verificador errado
    [InlineData("abcdefghijk")]   // letras
    public void IsValid_ShouldReturnFalse_WhenIsbnIsInvalid(string isbn)
    {
        var result = _validator.IsValid(isbn);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    // O validador não deve validar strings vazias ou nulas, pois isso é responsabilidade do [Required]
        public void IsValid_ShouldReturnTrue_WhenValueIsEmpty(string isbn)
    {
        var result = _validator.IsValid(isbn);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenIsbnIsNull()
    {
        // null é tratado pelo [Required], não pelo [Isbn]
        var result = _validator.IsValid(null);
        result.Should().BeTrue();
    }
}