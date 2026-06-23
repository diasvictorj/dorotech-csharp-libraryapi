using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(LibraryDbContext context)
    {
        // Authors
        if (!await context.Authors.AnyAsync())
        {
            var authors = new List<Author>
            {
                new() { Name = "Machado de Assis" },
                new() { Name = "Clarice Lispector" },
                new() { Name = "Jorge Amado" },
                new() { Name = "Guimarães Rosa" }
            };
            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }

        // Books
        if (!await context.Books.AnyAsync())
        {
            var machadoId = (await context.Authors.FirstAsync(a => a.Name == "Machado de Assis")).Id;
            var clariceId = (await context.Authors.FirstAsync(a => a.Name == "Clarice Lispector")).Id;
            var jorgeId = (await context.Authors.FirstAsync(a => a.Name == "Jorge Amado")).Id;
            var guimaraesId = (await context.Authors.FirstAsync(a => a.Name == "Guimarães Rosa")).Id;

            var books = new List<Book>
            {
                new() { Title = "Dom Casmurro", Isbn = "978-85-254-1909-1", PublicationYear = 1899, AuthorId = machadoId },
                new() { Title = "Memórias Póstumas de Brás Cubas", Isbn = "978-85-254-1910-7", PublicationYear = 1881, AuthorId = machadoId },
                new() { Title = "A Hora da Estrela", Isbn = "978-85-359-0108-9", PublicationYear = 1977, AuthorId = clariceId },
                new() { Title = "Perto do Coração Selvagem", Isbn = "978-85-359-0107-2", PublicationYear = 1943, AuthorId = clariceId },
                new() { Title = "Gabriela, Cravo e Canela", Isbn = "978-85-254-1911-4", PublicationYear = 1958, AuthorId = jorgeId },
                new() { Title = "Grande Sertão: Veredas", Isbn = "978-85-254-1912-1", PublicationYear = 1956, AuthorId = guimaraesId }
            };
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }

        // Admin user
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}