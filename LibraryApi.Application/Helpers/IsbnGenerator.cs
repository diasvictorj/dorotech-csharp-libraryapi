namespace LibraryApi.Application.Helpers;

public static class IsbnGenerator
{
    public static string GenerateIsbn13()
    {
        var random = new Random();

        // ISBN-13 começa com 978 ou 979
        var prefix = random.Next(0, 2) == 0 ? "978" : "979";
        var body = string.Concat(Enumerable.Range(0, 9)
            .Select(_ => random.Next(0, 10).ToString()));

        var digits = prefix + body;

        // Calcula o dígito verificador
        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += (digits[i] - '0') * (i % 2 == 0 ? 1 : 3);

        var checkDigit = (10 - sum % 10) % 10;

        return digits + checkDigit;
    }
}