using Microsoft.AspNetCore.Mvc;
using Fast.Core.Dtos.Tickets;
using Fast.Business.Interfaces;

namespace Fast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// POST /api/ticket/create - Create a new support ticket
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
            var result = await _ticketService.CreateTicketAsync(createTicketDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/ticket/{ticketId} - Get ticket details by ID
        /// </summary>
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicket([FromRoute] int ticketId)
        {
            var result = await _ticketService.GetTicketByIdAsync(ticketId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/ticket/list - Get all tickets (paginated)
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllTickets([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _ticketService.GetAllTicketsAsync(pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/ticket/response - Add response to a ticket
        /// </summary>
        [HttpPost("response")]
        public async Task<IActionResult> AddResponse([FromBody] TicketResponseDto responseDto)
        {
            var result = await _ticketService.AddResponseAsync(responseDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// PUT /api/ticket/{ticketId}/close - Close a ticket
        /// </summary>
        [HttpPut("{ticketId}/close")]
        public async Task<IActionResult> CloseTicket([FromRoute] int ticketId)
        {
            var result = await _ticketService.CloseTicketAsync(ticketId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
