namespace Fast.Core.Dtos.Orders
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public string OrderName { get; set; }
        public string Description { get; set; }
    }
}
