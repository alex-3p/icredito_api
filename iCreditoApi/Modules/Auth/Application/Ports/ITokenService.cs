using iCreditoApi.Modules.Auth.Domain.Entities;

namespace iCreditoApi.Modules.Auth.Application.Ports;

/// <summary>
/// Puerto para el servicio de tokens JWT
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
