using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("charge")]
    public async Task<IActionResult> ProcessPayment(int orderId)
    {
        try
        {
            var (paymentIntentId, message) = await _paymentService.ProcessPaymentAsync(orderId);
            return Ok(new { message, PaymentIntentId = paymentIntentId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundPayment(int orderId)
    {
        try
        {
            var (refundId, message) = await _paymentService.RefundPaymentAsync(orderId);
            return Ok(new { message, RefundId = refundId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
