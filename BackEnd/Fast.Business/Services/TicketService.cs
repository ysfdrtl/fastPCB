using Fast.Core.Dtos.Tickets;
using Fast.Core.Dtos.Common;
using Fast.Data.Context;
using Fast.Data.Entities;
using Fast.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fast.Business.Services
{
    public class TicketService : ITicketService
    {
        private readonly FastDbContext _context;

        public TicketService(FastDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<TicketDetailDto>> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(createTicketDto.UserId);
                if (user == null)
                {
                    return new ApiResponseDto<TicketDetailDto>
                    {
                        Success = false,
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                var ticket = new Ticket
                {
                    UserId = createTicketDto.UserId,
                    Subject = createTicketDto.Subject,
                    IssueDescription = createTicketDto.IssueDescription,
                    CreatedAt = DateTime.UtcNow,
                    Status = TicketStatus.Open
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                var response = MapTicketToDetailDto(ticket);

                return new ApiResponseDto<TicketDetailDto>
                {
                    Success = true,
                    Message = "Ticket created successfully",
                    Data = response,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TicketDetailDto>
                {
                    Success = false,
                    Message = $"Ticket creation failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<TicketDetailDto>> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Responses)
                    .FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket == null)
                {
                    return new ApiResponseDto<TicketDetailDto>
                    {
                        Success = false,
                        Message = "Ticket not found",
                        StatusCode = 404
                    };
                }

                var response = MapTicketToDetailDto(ticket);

                return new ApiResponseDto<TicketDetailDto>
                {
                    Success = true,
                    Message = "Ticket retrieved successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<TicketDetailDto>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<PaginationDto<TicketDetailDto>>> GetAllTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Tickets.CountAsync();
                var tickets = await _context.Tickets
                    .Include(t => t.Responses)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responseItems = tickets.Select(t => MapTicketToDetailDto(t)).ToList();

                var pagination = new PaginationDto<TicketDetailDto>
                {
                    Items = responseItems,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return new ApiResponseDto<PaginationDto<TicketDetailDto>>
                {
                    Success = true,
                    Message = "Tickets retrieved successfully",
                    Data = pagination,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PaginationDto<TicketDetailDto>>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> AddResponseAsync(TicketResponseDto responseDto)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(responseDto.TicketId);
                if (ticket == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Ticket not found",
                        StatusCode = 404
                    };
                }

                var response = new TicketResponse
                {
                    TicketId = responseDto.TicketId,
                    Response = responseDto.Response,
                    RespondedBy = responseDto.RespondedBy,
                    RespondedAt = DateTime.UtcNow,
                    IsAdminResponse = true
                };

                _context.TicketResponses.Add(response);

                // Update ticket status to InProgress
                ticket.Status = TicketStatus.InProgress;
                _context.Tickets.Update(ticket);

                await _context.SaveChangesAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Response added successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Failed to add response: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> CloseTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Ticket not found",
                        StatusCode = 404
                    };
                }

                ticket.Status = TicketStatus.Closed;
                ticket.ResolvedAt = DateTime.UtcNow;
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Ticket closed successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Failed to close ticket: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private TicketDetailDto MapTicketToDetailDto(Ticket ticket)
        {
            return new TicketDetailDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                IssueDescription = ticket.IssueDescription,
                Subject = ticket.Subject,
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                ResolvedAt = ticket.ResolvedAt,
                Responses = ticket.Responses?.Select(r => new TicketReplyDto
                {
                    Id = r.Id,
                    Response = r.Response,
                    RespondedBy = r.RespondedBy,
                    RespondedAt = r.RespondedAt,
                    IsAdminResponse = r.IsAdminResponse
                }).ToList() ?? new List<TicketReplyDto>()
            };
        }
    }
}
