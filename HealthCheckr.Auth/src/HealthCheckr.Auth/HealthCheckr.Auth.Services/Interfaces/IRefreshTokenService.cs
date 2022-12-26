using HealthCheckr.Auth.Common.Models;

namespace HealthCheckr.Auth.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResponse> RefreshTokens();
        Task SaveTokens(RefreshTokenResponse tokens);
    }
}
