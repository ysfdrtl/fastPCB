namespace FastPCB.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int? Layers { get; set; }
        public string? Material { get; set; }
        public double? MinDistance { get; set; }
        public int? Quantity { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Ticket> Reports { get; set; } = new List<Ticket>();
        public ICollection<ProjectLike> Likes { get; set; } = new List<ProjectLike>();
    }

    public enum ProjectStatus
    {
        Draft = 0,
        Published = 1,
        Featured = 2,
        Archived = 3,
        Removed = 4
    }
}
