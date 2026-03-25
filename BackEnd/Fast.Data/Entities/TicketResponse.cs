namespace Fast.Data.Entities
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Response { get; set; }
        public string RespondedBy { get; set; } // Admin email or name
        public DateTime RespondedAt { get; set; }
        public bool IsAdminResponse { get; set; }

        // Navigation property
        public Ticket Ticket { get; set; }
    }
}
