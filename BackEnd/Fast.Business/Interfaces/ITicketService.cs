using Fast.Core.Dtos.Tickets;
using Fast.Core.Dtos.Common;

namespace Fast.Business.Interfaces
{
    public interface ITicketService
    {
        Task<ApiResponseDto<TicketDetailDto>> CreateTicketAsync(CreateTicketDto createTicketDto);
        Task<ApiResponseDto<TicketDetailDto>> GetTicketByIdAsync(int ticketId);
        Task<ApiResponseDto<PaginationDto<TicketDetailDto>>> GetAllTicketsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponseDto> AddResponseAsync(TicketResponseDto responseDto);
        Task<ApiResponseDto> CloseTicketAsync(int ticketId);
    }
}
