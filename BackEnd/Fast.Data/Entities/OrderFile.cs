namespace Fast.Data.Entities
{
    public class OrderFile
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; } // gerber, zip, pdf, etc.
        public long FileSize { get; set; } // Size in bytes
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } // User email or ID

        // Navigation property
        public Order Order { get; set; }
    }
}
