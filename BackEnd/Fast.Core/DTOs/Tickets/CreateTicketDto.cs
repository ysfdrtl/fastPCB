namespace Fast.Core.Dtos.Tickets
{
    public class CreateTicketDto
    {
        public int UserId { get; set; }
        public string IssueDescription { get; set; }
        public string Subject { get; set; }
        public int? OrderId { get; set; }
    }
}
