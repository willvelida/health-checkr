using Azure.Security.KeyVault.Secrets;

namespace HealthCheckr.Auth.Repository.Interfaces
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> GetSecret(string secretName);
        Task SaveSecret(string secretName, string secretValue);
    }
}
