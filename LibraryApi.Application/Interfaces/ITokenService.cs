using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}