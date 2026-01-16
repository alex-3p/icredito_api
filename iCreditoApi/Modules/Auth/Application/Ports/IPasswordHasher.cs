namespace iCreditoApi.Modules.Auth.Application.Ports;

/// <summary>
/// Puerto para el servicio de hashing de contraseñas
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
