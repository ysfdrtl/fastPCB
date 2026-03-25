namespace Fast.Core.Dtos.Tickets
{
    public class TicketResponseDto
    {
        public int TicketId { get; set; }
        public string Response { get; set; }
        public string RespondedBy { get; set; } // Admin email or name
    }

    public class TicketDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IssueDescription { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; } // Open, InProgress, Resolved, Closed
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<TicketReplyDto> Responses { get; set; }
    }

    public class TicketReplyDto
    {
        public int Id { get; set; }
        public string Response { get; set; }
        public string RespondedBy { get; set; }
        public DateTime RespondedAt { get; set; }
        public bool IsAdminResponse { get; set; }
    }
}
