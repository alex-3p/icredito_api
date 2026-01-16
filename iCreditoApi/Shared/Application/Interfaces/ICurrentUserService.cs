namespace iCreditoApi.Shared.Application.Interfaces;

/// <summary>
/// Servicio para obtener información del usuario actual autenticado
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
}
