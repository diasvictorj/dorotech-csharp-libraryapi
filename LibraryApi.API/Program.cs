using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Application.Interfaces;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dependency Injection - Book module
// Registers the contract (interface) and its implementation.
// Scoped: a new instance per HTTP request.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();