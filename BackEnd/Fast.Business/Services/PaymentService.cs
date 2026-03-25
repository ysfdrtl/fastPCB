using Fast.Core.Dtos.Orders;
using Fast.Core.Dtos.Common;
using Fast.Data.Context;
using Fast.Data.Entities;
using Fast.Business.Interfaces;

namespace Fast.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly FastDbContext _context;

        public PaymentService(FastDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> ProcessPaymentAsync(PaymentDto paymentDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(paymentDto.OrderId);
                if (order == null)
                {
                    return new ApiResponseDto<PaymentResponseDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                // Simple payment processing logic - in real scenario, integrate with payment gateway
                var transactionId = Guid.NewGuid().ToString();

                var payment = new Payment
                {
                    OrderId = paymentDto.OrderId,
                    Amount = paymentDto.Amount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    TransactionId = transactionId,
                    Status = PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                var response = MapPaymentToDto(payment);

                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment processed successfully",
                    Data = response,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = false,
                    Message = $"Payment processing failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> VerifyPaymentAsync(PaymentVerificationDto verificationDto)
        {
            try
            {
                var payment = _context.Payments
                    .FirstOrDefault(p => p.OrderId == verificationDto.OrderId && p.TransactionId == verificationDto.TransactionId);

                if (payment == null)
                {
                    return new ApiResponseDto<PaymentResponseDto>
                    {
                        Success = false,
                        Message = "Payment not found",
                        StatusCode = 404
                    };
                }

                // Verify payment with gateway - for demo purposes, mark as completed
                payment.Status = PaymentStatus.Completed;
                payment.ConfirmedDate = DateTime.UtcNow;

                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                var response = MapPaymentToDto(payment);

                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment verified successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = false,
                    Message = $"Payment verification failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                var payment = _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
                if (payment == null)
                {
                    return new ApiResponseDto<PaymentResponseDto>
                    {
                        Success = false,
                        Message = "Payment not found",
                        StatusCode = 404
                    };
                }

                var response = MapPaymentToDto(payment);

                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment retrieved successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PaymentResponseDto>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> RefundPaymentAsync(int paymentId)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(paymentId);
                if (payment == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Payment not found",
                        StatusCode = 404
                    };
                }

                payment.Status = PaymentStatus.Refunded;
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Payment refunded successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Refund failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private PaymentResponseDto MapPaymentToDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                Status = payment.Status.ToString(),
                PaymentDate = payment.PaymentDate,
                ConfirmedDate = payment.ConfirmedDate
            };
        }
    }
}
