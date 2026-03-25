namespace Fast.Core.Dtos.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string TrackingNumber { get; set; }
        public List<OrderFileDto> Files { get; set; }
        public OrderSpecificationDto Specifications { get; set; }
        public PaymentResponseDto Payment { get; set; }
    }
}
