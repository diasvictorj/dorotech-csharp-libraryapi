# 📚 Library API

API RESTful para gerenciamento do acervo de uma livraria, desenvolvida em .NET 10 com C#.

Autor: **João Victor da Silva Dias** — [github.com/diasvictorj](https://github.com/diasvictorj)

---

## 🚀 Tecnologias

- .NET 10 / C#
- ASP.NET Core Web API
- Entity Framework Core 9.x (compatibilidade com Pomelo/MySQL)
- MySQL + Pomelo.EntityFrameworkCore.MySql
- JWT Bearer Authentication
- Swagger / Swashbuckle (OpenAPI 2.x)
- Serilog (logs em console e arquivo)
- BCrypt.Net para hash de senhas

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizado em 4 camadas com dependências unidirecionais:

```
LibraryApi.Domain          → entidades e interfaces (sem dependências externas)
LibraryApi.Application     → regras de negócio, DTOs, services
LibraryApi.Infrastructure  → EF Core, repositórios, TokenService
LibraryApi.API             → controllers, middlewares, configuração
```

A regra fundamental: **Domain não conhece ninguém. Infrastructure e Application conhecem Domain. API conhece todos.**

---

## ⚙️ Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL 8+ rodando localmente
- `dotnet-ef` tool instalado globalmente:

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

---

## 🔧 Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/diasvictorj/library-api.git
cd library-api
```

### 2. Crie o banco de dados

```sql
CREATE DATABASE libraryapi;
CREATE USER 'library_user'@'localhost' IDENTIFIED BY 'SenhaForte123!';
GRANT ALL PRIVILEGES ON libraryapi.* TO 'library_user'@'localhost';
FLUSH PRIVILEGES;
```

### 3. Configure os secrets locais

```bash
cd LibraryApi.API

dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=libraryapi;User=library_user;Password=SenhaForte123!;"

dotnet user-secrets set "Jwt:Key" "uma-chave-bem-grande-e-aleatoria-com-pelo-menos-32-caracteres"

cd ..
```

> Os secrets nunca vão para o repositório. O `appsettings.json` contém apenas a estrutura de configuração, sem valores sensíveis.

### 4. Suba a aplicação

```bash
cd LibraryApi.API
dotnet run
```

As migrations são aplicadas automaticamente ao subir. Um seed de dados inicial também é executado, criando autores, livros e o usuário administrador.

---

## 🌱 Seed de dados

Ao iniciar pela primeira vez, a aplicação popula o banco automaticamente com:

**Autores:** Machado de Assis, Clarice Lispector, Jorge Amado, Guimarães Rosa

**Livros:** Dom Casmurro, Memórias Póstumas de Brás Cubas, A Hora da Estrela, Perto do Coração Selvagem, Gabriela Cravo e Canela, Grande Sertão: Veredas

**Usuário admin:**
- Username: `admin`
- Password: `Admin@123`

---

## 📖 Documentação da API (Swagger)

Acesse após subir a aplicação:

```
http://localhost:5104
```

### Autenticação no Swagger

1. `POST /api/auth/login` com as credenciais do admin
2. Copie o token retornado
3. Clique no botão **Authorize** (🔒) e cole: `Bearer SEU_TOKEN`

---

## 🔐 Níveis de acesso

| Operação | Acesso |
|---|---|
| `GET /api/books` | Público (sem autenticação) |
| `GET /api/books/{id}` | Público (sem autenticação) |
| `POST /api/books` | Admin (Bearer JWT) |
| `PUT /api/books/{id}` | Admin (Bearer JWT) |
| `DELETE /api/books/{id}` | Admin (Bearer JWT) |
| `POST /api/auth/login` | Público |

---

## 🔍 Filtros e paginação

O endpoint `GET /api/books` suporta os seguintes parâmetros de query:

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `title` | string | Filtra por título (busca parcial) |
| `authorName` | string | Filtra por nome do autor (busca parcial) |
| `publicationYear` | int | Filtra por ano de publicação |
| `page` | int | Página atual (padrão: 1) |
| `pageSize` | int | Itens por página (padrão: 10, máximo: 50) |

Exemplo:
```
GET /api/books?authorName=Machado&page=1&pageSize=2
```

---

## 📋 Códigos de resposta

| Código | Situação |
|---|---|
| `200 OK` | Consulta realizada com sucesso |
| `201 Created` | Livro cadastrado com sucesso |
| `204 No Content` | Atualização ou exclusão realizada |
| `400 Bad Request` | Dados de entrada inválidos |
| `401 Unauthorized` | Token ausente ou inválido |
| `404 Not Found` | Recurso não encontrado |
| `409 Conflict` | ISBN duplicado ou autor inexistente |
| `500 Internal Server Error` | Erro inesperado (logado via Serilog) |

---

## 📝 Logs

Os logs são gerados via **Serilog** em dois destinos:

- **Console** — visível ao rodar `dotnet run`
- **Arquivo** — em `logs/library-api-YYYYMMDD.log` (rotação diária)

---

## 🗄️ Scripts SQL

O script de criação das tabelas está disponível em:

```
Scripts/migration.sql
```

As migrations também são aplicadas automaticamente ao iniciar a aplicação.

---

## 🔩 Notas técnicas

- Projeto desenvolvido em **.NET 10**.
- Os pacotes `Microsoft.EntityFrameworkCore` e `Microsoft.EntityFrameworkCore.Design` estão fixados na versão **9.0.0** por compatibilidade com o driver `Pomelo.EntityFrameworkCore.MySql`, que ainda não possui release estável para EF Core 10 no momento do desenvolvimento. Isso não afeta o funcionamento da aplicação, que roda em .NET 10.
- O pacote `Microsoft.AspNetCore.Authentication.JwtBearer` está fixado na versão **9.0.0** pelo mesmo motivo de consistência de dependências transitivas.
- A versão **2.x** do `Microsoft.OpenApi` (puxada pelo Swashbuckle 10.x) introduziu breaking changes na API de configuração do `OpenApiSecurityRequirement`. A sintaxe utilizada neste projeto está adaptada para essa versão.

---

## 📁 Estrutura do projeto

```
LibraryApi/
├── LibraryApi.API/
│   ├── Controllers/        # BooksController, AuthController
│   ├── Middlewares/        # ErrorHandlingMiddleware
│   ├── logs/               # Logs gerados pelo Serilog
│   └── Program.cs
├── LibraryApi.Application/
│   ├── DTOs/               # BookDto, CreateBookDto, LoginDto, etc.
│   ├── Exceptions/         # BusinessException
│   ├── Interfaces/         # IBookService, IAuthService, ITokenService
│   └── Services/           # BookService, AuthService
├── LibraryApi.Domain/
│   ├── Entities/           # Book, Author, User
│   ├── Enums/              # UserRole
│   └── Interfaces/         # IBookRepository, IAuthorRepository, IUserRepository
├── LibraryApi.Infrastructure/
│   ├── Data/               # LibraryDbContext, DataSeeder
│   ├── Migrations/         # EF Core migrations
│   ├── Repositories/       # BookRepository, AuthorRepository, UserRepository
│   └── Services/           # TokenService
└── Scripts/
    └── migration.sql       # Script SQL gerado pelas migrations
```