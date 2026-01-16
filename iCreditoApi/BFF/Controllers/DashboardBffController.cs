using iCreditoApi.BFF.Services;
using iCreditoApi.BFF.ViewModels;
using iCreditoApi.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCreditoApi.BFF.Controllers;

[ApiController]
[Route("api/bff")]
[Authorize]
public class DashboardBffController : ControllerBase
{
    private readonly DashboardAggregator _dashboardAggregator;
    private readonly ICurrentUserService _currentUser;

    public DashboardBffController(
        DashboardAggregator dashboardAggregator,
        ICurrentUserService currentUser)
    {
        _dashboardAggregator = dashboardAggregator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtiene el dashboard completo del usuario
    /// Datos optimizados y agregados para el frontend
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var result = await _dashboardAggregator.GetDashboardAsync(_currentUser.UserId, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene los KPIs del usuario
    /// Métricas financieras, de tarjetas, transacciones y tendencias
    /// </summary>
    [HttpGet("kpis")]
    [ProducesResponseType(typeof(KpisViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKpis(CancellationToken ct)
    {
        var result = await _dashboardAggregator.GetKpisAsync(_currentUser.UserId, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }
}
