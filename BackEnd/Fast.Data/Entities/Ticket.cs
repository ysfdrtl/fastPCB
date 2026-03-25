namespace Fast.Data.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Subject { get; set; }
        public string IssueDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public ICollection<TicketResponse> Responses { get; set; } = new List<TicketResponse>();
    }

    public enum TicketStatus
    {
        Open = 0,
        InProgress = 1,
        Resolved = 2,
        Closed = 3
    }
}
