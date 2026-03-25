namespace Fast.Core.Dtos.Orders
{
    public class OrderFileDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
    }

    public class UploadFileDto
    {
        public int OrderId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
    }
}
