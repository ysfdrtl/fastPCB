namespace Fast.Core.Dtos.Orders
{
    public class PaymentDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Credit Card, Bank Transfer, etc.
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed
        public DateTime PaymentDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
    }

    public class PaymentVerificationDto
    {
        public int OrderId { get; set; }
        public string TransactionId { get; set; }
    }
}
