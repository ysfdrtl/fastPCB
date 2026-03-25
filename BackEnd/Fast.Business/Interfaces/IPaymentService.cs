using Fast.Core.Dtos.Orders;
using Fast.Core.Dtos.Common;

namespace Fast.Business.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponseDto<PaymentResponseDto>> ProcessPaymentAsync(PaymentDto paymentDto);
        Task<ApiResponseDto<PaymentResponseDto>> VerifyPaymentAsync(PaymentVerificationDto verificationDto);
        Task<ApiResponseDto<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId);
        Task<ApiResponseDto> RefundPaymentAsync(int paymentId);
    }
}
