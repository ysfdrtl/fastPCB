using Fast.Core.Dtos.Orders;
using Fast.Core.Dtos.Common;

namespace Fast.Business.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<ApiResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId);
        Task<ApiResponseDto<PaginationDto<OrderResponseDto>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponseDto> DeleteOrderAsync(int orderId);
        Task<ApiResponseDto<OrderFileDto>> UploadFileAsync(int orderId, UploadFileDto uploadFileDto);
        Task<ApiResponseDto<OrderResponseDto>> AddOrderSpecificationAsync(int orderId, OrderSpecificationDto specificationDto);
        Task<ApiResponseDto<PriceCalculationDto>> CalculatePriceAsync(int orderId);
        Task<ApiResponseDto<OrderTrackingDto>> GetOrderTrackingAsync(int orderId);
        Task<ApiResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateStatusDto);
    }
}
