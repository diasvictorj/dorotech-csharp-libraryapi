using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    void Revoke(RefreshToken refreshToken);
    Task<bool> SaveChangesAsync();
}