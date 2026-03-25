using Fast.Core.Dtos.Orders;
using Fast.Core.Dtos.Common;
using Fast.Data.Context;
using Fast.Data.Entities;
using Fast.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fast.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly FastDbContext _context;

        public OrderService(FastDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(createOrderDto.UserId);
                if (user == null)
                {
                    return new ApiResponseDto<OrderResponseDto>
                    {
                        Success = false,
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                var order = new Order
                {
                    UserId = createOrderDto.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Draft,
                    TotalAmount = 0
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var response = MapOrderToResponseDto(order);

                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = true,
                    Message = "Order created successfully",
                    Data = response,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Order creation failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Files)
                    .Include(o => o.Specifications)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ApiResponseDto<OrderResponseDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                var response = MapOrderToResponseDto(order);

                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = true,
                    Message = "Order retrieved successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<PaginationDto<OrderResponseDto>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Orders.CountAsync();
                var orders = await _context.Orders
                    .Include(o => o.Files)
                    .Include(o => o.Specifications)
                    .Include(o => o.Payment)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responseItems = orders.Select(o => MapOrderToResponseDto(o)).ToList();

                var pagination = new PaginationDto<OrderResponseDto>
                {
                    Items = responseItems,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return new ApiResponseDto<PaginationDto<OrderResponseDto>>
                {
                    Success = true,
                    Message = "Orders retrieved successfully",
                    Data = pagination,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PaginationDto<OrderResponseDto>>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                // Only allow deletion if order is in Draft status
                if (order.Status != OrderStatus.Draft)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Only draft orders can be deleted",
                        StatusCode = 400
                    };
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Order deleted successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Deletion failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<OrderFileDto>> UploadFileAsync(int orderId, UploadFileDto uploadFileDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new ApiResponseDto<OrderFileDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                var orderFile = new OrderFile
                {
                    OrderId = orderId,
                    FileName = uploadFileDto.FileName,
                    FileUrl = uploadFileDto.FileUrl,
                    FileType = uploadFileDto.FileType,
                    FileSize = uploadFileDto.FileSize,
                    UploadDate = DateTime.UtcNow,
                    UploadedBy = "User"
                };

                _context.OrderFiles.Add(orderFile);
                await _context.SaveChangesAsync();

                var response = new OrderFileDto
                {
                    Id = orderFile.Id,
                    OrderId = orderFile.OrderId,
                    FileName = orderFile.FileName,
                    FileUrl = orderFile.FileUrl,
                    FileType = orderFile.FileType,
                    FileSize = orderFile.FileSize,
                    UploadDate = orderFile.UploadDate,
                    UploadedBy = orderFile.UploadedBy
                };

                return new ApiResponseDto<OrderFileDto>
                {
                    Success = true,
                    Message = "File uploaded successfully",
                    Data = response,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderFileDto>
                {
                    Success = false,
                    Message = $"File upload failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<OrderResponseDto>> AddOrderSpecificationAsync(int orderId, OrderSpecificationDto specificationDto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Specifications)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ApiResponseDto<OrderResponseDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                var specification = new OrderSpecification
                {
                    OrderId = orderId,
                    LayerCount = specificationDto.LayerCount,
                    MinimumSpacing = specificationDto.MinimumSpacing,
                    Material = specificationDto.Material,
                    BoardWidth = specificationDto.BoardWidth,
                    BoardHeight = specificationDto.BoardHeight,
                    CopperWeight = specificationDto.CopperWeight,
                    SolderMask = specificationDto.SolderMask,
                    Silkscreen = specificationDto.Silkscreen,
                    HasVias = specificationDto.HasVias,
                    HasPlatedHoles = specificationDto.HasPlatedHoles,
                    SurfaceFinish = specificationDto.SurfaceFinish,
                    Quantity = specificationDto.Quantity,
                    Notes = specificationDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderSpecifications.Add(specification);
                await _context.SaveChangesAsync();

                var response = MapOrderToResponseDto(order);

                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = true,
                    Message = "Specification added successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Failed to add specification: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<PriceCalculationDto>> CalculatePriceAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Specifications)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ApiResponseDto<PriceCalculationDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                var spec = order.Specifications.FirstOrDefault();
                if (spec == null)
                {
                    return new ApiResponseDto<PriceCalculationDto>
                    {
                        Success = false,
                        Message = "Order specifications not found",
                        StatusCode = 404
                    };
                }

                // Simple price calculation logic
                decimal basePrice = 50m; // Base price
                decimal layerCost = spec.LayerCount * 10m;
                decimal quantityCost = spec.Quantity * 5m;
                decimal materialCost = 20m; // Fixed material cost for demo

                decimal totalPrice = basePrice + layerCost + quantityCost + materialCost;
                decimal discount = totalPrice > 500m ? totalPrice * 0.1m : 0m;
                decimal finalPrice = totalPrice - discount;

                var response = new PriceCalculationDto
                {
                    OrderId = orderId,
                    BasePrice = basePrice,
                    LayerCost = layerCost,
                    QuantityCost = quantityCost,
                    MaterialCost = materialCost,
                    TotalPrice = totalPrice,
                    Discount = discount,
                    FinalPrice = finalPrice
                };

                order.TotalAmount = finalPrice;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return new ApiResponseDto<PriceCalculationDto>
                {
                    Success = true,
                    Message = "Price calculated successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PriceCalculationDto>
                {
                    Success = false,
                    Message = $"Price calculation failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<OrderTrackingDto>> GetOrderTrackingAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.StatusHistory)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ApiResponseDto<OrderTrackingDto>
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                var response = new OrderTrackingDto
                {
                    OrderId = order.Id,
                    TrackingNumber = order.TrackingNumber,
                    CurrentStatus = order.Status.ToString(),
                    LastUpdated = order.StatusHistory.OrderByDescending(h => h.ChangedDate).FirstOrDefault()?.ChangedDate ?? order.OrderDate,
                    StatusHistory = order.StatusHistory.OrderByDescending(h => h.ChangedDate).Select(h => new OrderStatusHistoryDto
                    {
                        Id = h.Id,
                        Status = h.Status.ToString(),
                        ChangedDate = h.ChangedDate,
                        Notes = h.Notes,
                        UpdatedBy = h.UpdatedBy
                    }).ToList()
                };

                return new ApiResponseDto<OrderTrackingDto>
                {
                    Success = true,
                    Message = "Tracking information retrieved successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<OrderTrackingDto>
                {
                    Success = false,
                    Message = $"Tracking retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateStatusDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(updateStatusDto.OrderId);
                if (order == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Order not found",
                        StatusCode = 404
                    };
                }

                if (Enum.TryParse<OrderTrackingStatus>(updateStatusDto.Status, out var newStatus))
                {
                    var statusHistory = new OrderStatusHistory
                    {
                        OrderId = updateStatusDto.OrderId,
                        Status = newStatus,
                        ChangedDate = DateTime.UtcNow,
                        Notes = updateStatusDto.Notes,
                        UpdatedBy = "Admin"
                    };

                    _context.OrderStatusHistories.Add(statusHistory);

                    if (!string.IsNullOrEmpty(updateStatusDto.TrackingNumber))
                    {
                        order.TrackingNumber = updateStatusDto.TrackingNumber;
                    }

                    await _context.SaveChangesAsync();

                    return new ApiResponseDto
                    {
                        Success = true,
                        Message = "Order status updated successfully",
                        StatusCode = 200
                    };
                }

                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Invalid status value",
                    StatusCode = 400
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Status update failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private OrderResponseDto MapOrderToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                TrackingNumber = order.TrackingNumber,
                Files = order.Files?.Select(f => new OrderFileDto
                {
                    Id = f.Id,
                    OrderId = f.OrderId,
                    FileName = f.FileName,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType,
                    FileSize = f.FileSize,
                    UploadDate = f.UploadDate,
                    UploadedBy = f.UploadedBy
                }).ToList() ?? new List<OrderFileDto>(),
                Specifications = order.Specifications?.FirstOrDefault() != null ? MapSpecificationToDto(order.Specifications.First()) : null,
                Payment = order.Payment != null ? MapPaymentToDto(order.Payment) : null
            };
        }

        private OrderSpecificationDto MapSpecificationToDto(OrderSpecification spec)
        {
            return new OrderSpecificationDto
            {
                Id = spec.Id,
                OrderId = spec.OrderId,
                LayerCount = spec.LayerCount,
                MinimumSpacing = spec.MinimumSpacing,
                Material = spec.Material,
                BoardWidth = spec.BoardWidth,
                BoardHeight = spec.BoardHeight,
                CopperWeight = spec.CopperWeight,
                SolderMask = spec.SolderMask,
                Silkscreen = spec.Silkscreen,
                HasVias = spec.HasVias,
                HasPlatedHoles = spec.HasPlatedHoles,
                SurfaceFinish = spec.SurfaceFinish,
                Quantity = spec.Quantity,
                Notes = spec.Notes,
                CreatedAt = spec.CreatedAt,
                UpdatedAt = spec.UpdatedAt
            };
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
