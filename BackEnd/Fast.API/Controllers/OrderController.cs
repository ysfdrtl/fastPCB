using Microsoft.AspNetCore.Mvc;
using Fast.Core.Dtos.Orders;
using Fast.Business.Interfaces;

namespace Fast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// POST /api/order/create - Create a new order
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var result = await _orderService.CreateOrderAsync(createOrderDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/order/{orderId} - Get order details by ID
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder([FromRoute] int orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/order/list - Get all orders (paginated)
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrdersAsync(pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// DELETE /api/order/{orderId} - Delete order (only Draft orders)
        /// </summary>
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/order/upload - Upload gerber file
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileName) || string.IsNullOrWhiteSpace(request.FileBase64))
                return BadRequest("FileName and FileBase64 are required");

            var byteArray = Convert.FromBase64String(request.FileBase64);
            var uploadFileDto = new UploadFileDto
            {
                OrderId = request.OrderId,
                FileName = request.FileName,
                FileUrl = $"/uploads/{request.OrderId}/{request.FileName}",
                FileType = Path.GetExtension(request.FileName),
                FileSize = byteArray.Length
            };

            var result = await _orderService.UploadFileAsync(request.OrderId, uploadFileDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/order/specification - Add PCB specifications
        /// </summary>
        [HttpPost("specification")]
        public async Task<IActionResult> AddSpecification([FromBody] OrderSpecificationDto specDto, [FromQuery] int orderId)
        {
            var result = await _orderService.AddOrderSpecificationAsync(orderId, specDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/order/price - Calculate order price
        /// </summary>
        [HttpPost("price")]
        public async Task<IActionResult> CalculatePrice([FromQuery] int orderId)
        {
            var result = await _orderService.CalculatePriceAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/order/tracking/{orderId} - Get order tracking/status history
        /// </summary>
        [HttpGet("tracking/{orderId}")]
        public async Task<IActionResult> GetOrderTracking([FromRoute] int orderId)
        {
            var result = await _orderService.GetOrderTrackingAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// PUT /api/order/status - Update order status
        /// </summary>
        [HttpPut("status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(updateStatusDto);
            return StatusCode((int)result.StatusCode, result);
        }
    }

    public class UploadFileRequest
    {
        public int OrderId { get; set; }
        public string? FileName { get; set; }
        public string? FileBase64 { get; set; }
    }
}
