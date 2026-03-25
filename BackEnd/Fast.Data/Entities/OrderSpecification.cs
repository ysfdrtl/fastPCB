namespace Fast.Data.Entities
{
    public class OrderSpecification
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        // PCB Technical Specifications
        public int LayerCount { get; set; } // Number of layers
        public double MinimumSpacing { get; set; } // Minimum spacing between traces
        public string Material { get; set; } // FR-4, Polyimide, etc.
        public double BoardWidth { get; set; } // mm
        public double BoardHeight { get; set; } // mm
        public string CopperWeight { get; set; } // 1oz, 2oz, etc.
        public string SolderMask { get; set; } // Color: Green, Red, Blue, Black, White
        public string Silkscreen { get; set; } // Color
        public bool HasVias { get; set; }
        public bool HasPlatedHoles { get; set; }
        public string SurfaceFinish { get; set; } // HASL, ENIG, OSP, etc.
        public int Quantity { get; set; } // Number of PCBs
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Notes { get; set; }

        // Navigation property
        public Order Order { get; set; }
    }
}
