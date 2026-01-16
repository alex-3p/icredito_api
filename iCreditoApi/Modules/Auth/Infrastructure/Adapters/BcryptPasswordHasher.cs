using iCreditoApi.Modules.Auth.Application.Ports;

namespace iCreditoApi.Modules.Auth.Infrastructure.Adapters;

/// <summary>
/// Implementación del hasher de contraseñas usando BCrypt
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
