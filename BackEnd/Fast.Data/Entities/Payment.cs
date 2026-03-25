namespace Fast.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Credit Card, Bank Transfer, etc.
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; } // Pending, Completed, Failed
        public DateTime PaymentDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }

        // Navigation property
        public Order Order { get; set; }
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3
    }
}
