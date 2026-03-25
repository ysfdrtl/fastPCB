namespace Fast.Core.Dtos.Orders
{
    public class OrderTrackingDto
    {
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string CurrentStatus { get; set; } // Review, Approved, Production, QualityCheck, Packaging, Shipping, Delivered
        public DateTime LastUpdated { get; set; }
        public List<OrderStatusHistoryDto> StatusHistory { get; set; }
    }

    public class OrderStatusHistoryDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime ChangedDate { get; set; }
        public string Notes { get; set; }
        public string UpdatedBy { get; set; }
    }
}
