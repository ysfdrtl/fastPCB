namespace Fast.Core.Dtos.Orders
{
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } // Review, Approved, Production, QualityCheck, Packaging, Shipping, Delivered, Cancelled
        public string Notes { get; set; }
        public string TrackingNumber { get; set; }
    }
}
