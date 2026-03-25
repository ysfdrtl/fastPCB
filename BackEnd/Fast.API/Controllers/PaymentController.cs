using Microsoft.AspNetCore.Mvc;
using Fast.Core.Dtos.Orders;
using Fast.Business.Interfaces;

namespace Fast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// POST /api/payment/process - Process a payment for an order
        /// </summary>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDto paymentDto)
        {
            var result = await _paymentService.ProcessPaymentAsync(paymentDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/payment/verify - Verify payment completion
        /// </summary>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationDto verificationDto)
        {
            var result = await _paymentService.VerifyPaymentAsync(verificationDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/payment/order/{orderId} - Get payment details by order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId([FromRoute] int orderId)
        {
            var result = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/payment/refund - Refund a payment
        /// </summary>
        [HttpPost("refund/{paymentId}")]
        public async Task<IActionResult> RefundPayment([FromRoute] int paymentId)
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
