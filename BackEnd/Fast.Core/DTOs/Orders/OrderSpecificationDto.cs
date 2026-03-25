namespace Fast.Core.Dtos.Orders
{
    public class OrderSpecificationDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int LayerCount { get; set; }
        public double MinimumSpacing { get; set; }
        public string Material { get; set; }
        public double BoardWidth { get; set; }
        public double BoardHeight { get; set; }
        public string CopperWeight { get; set; }
        public string SolderMask { get; set; }
        public string Silkscreen { get; set; }
        public bool HasVias { get; set; }
        public bool HasPlatedHoles { get; set; }
        public string SurfaceFinish { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
