using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Application.Interfaces;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Infrastructure.Repositories;
using System.Text;
using LibraryApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.OpenApi;
using LibraryApi.API.Middlewares;



Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/library-api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

//DB Context configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dependency Injection - Book module
// Registers the contract (interface) and its implementation.
// Scoped: a new instance per HTTP request.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

// Dependency Injection - Auth module
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "API para gerenciamento de livros com autenticação JWT."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT. Exemplo: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// Data Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}


// Swagger UI — disponível sempre (inclusive em produção, útil para o teste do avaliador)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    options.RoutePrefix = string.Empty; // abre na raiz: http://localhost:5104
});

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


