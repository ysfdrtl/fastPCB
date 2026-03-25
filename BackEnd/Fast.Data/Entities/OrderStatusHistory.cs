namespace Fast.Data.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderTrackingStatus Status { get; set; }
        public DateTime ChangedDate { get; set; }
        public string Notes { get; set; }
        public string UpdatedBy { get; set; } // Admin who made the change

        // Navigation property
        public Order Order { get; set; }
    }

    public enum OrderTrackingStatus
    {
        Review = 0,          // Order under review
        Approved = 1,        // Order approved
        Production = 2,      // In production
        QualityCheck = 3,    // Quality check
        Packaging = 4,       // Being packaged
        Shipping = 5,        // In transit
        Delivered = 6,       // Delivered
        Cancelled = 7        // Order cancelled
    }
}
