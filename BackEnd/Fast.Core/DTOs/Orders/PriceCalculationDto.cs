namespace Fast.Core.Dtos.Orders
{
    public class PriceCalculationDto
    {
        public int OrderId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal LayerCost { get; set; }
        public decimal QuantityCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
