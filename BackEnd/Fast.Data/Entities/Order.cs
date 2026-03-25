namespace Fast.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string TrackingNumber { get; set; }
        public ICollection<OrderFile> Files { get; set; } = new List<OrderFile>();
        public ICollection<OrderSpecification> Specifications { get; set; } = new List<OrderSpecification>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        public Payment Payment { get; set; }
    }

    public enum OrderStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Cancelled = 3
    }
}