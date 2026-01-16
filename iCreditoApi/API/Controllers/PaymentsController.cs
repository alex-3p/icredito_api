using iCreditoApi.Modules.Payments.Application.DTOs;
using iCreditoApi.Modules.Payments.Application.Services;
using iCreditoApi.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCreditoApi.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly ICurrentUserService _currentUser;

    public PaymentsController(PaymentService paymentService, ICurrentUserService currentUser)
    {
        _paymentService = paymentService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtiene los pagos del usuario
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaymentListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _paymentService.GetUserPaymentsAsync(
            _currentUser.UserId, page, pageSize, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el detalle de un pago
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(Guid id, CancellationToken ct)
    {
        var result = await _paymentService.GetByIdAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Procesa un nuevo pago
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment(
        [FromBody] ProcessPaymentRequest request,
        CancellationToken ct)
    {
        var result = await _paymentService.ProcessPaymentAsync(
            _currentUser.UserId, request, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = result.Value.PaymentId },
            result.Value);
    }

    /// <summary>
    /// Reembolsa un pago
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefundPayment(Guid id, CancellationToken ct)
    {
        var result = await _paymentService.RefundPaymentAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }
}
