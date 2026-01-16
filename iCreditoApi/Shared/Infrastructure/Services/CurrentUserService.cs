using System.Security.Claims;
using iCreditoApi.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace iCreditoApi.Shared.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de usuario actual usando HttpContext
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User
        .FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User
        .Identity?.IsAuthenticated ?? false;
}
