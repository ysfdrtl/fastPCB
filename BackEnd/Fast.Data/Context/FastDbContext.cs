using Microsoft.EntityFrameworkCore;
using Fast.Data.Entities;

namespace Fast.Data.Context
{
    public class FastDbContext : DbContext
    {
        public FastDbContext(DbContextOptions<FastDbContext> options) : base(options)
        {
        }

        // DbSet properties for entities
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderSpecification> OrderSpecifications { get; set; }
        public DbSet<OrderFile> OrderFiles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketResponse> TicketResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.TrackingNumber).HasMaxLength(100);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderSpecification configuration
            modelBuilder.Entity<OrderSpecification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Material).HasMaxLength(100);
                entity.Property(e => e.CopperWeight).HasMaxLength(50);
                entity.Property(e => e.SolderMask).HasMaxLength(50);
                entity.Property(e => e.Silkscreen).HasMaxLength(50);
                entity.Property(e => e.SurfaceFinish).HasMaxLength(50);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Specifications)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderFile configuration
            modelBuilder.Entity<OrderFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FileUrl).IsRequired();
                entity.Property(e => e.FileType).HasMaxLength(50);
                entity.Property(e => e.UploadedBy).HasMaxLength(255);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Files)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentMethod).HasMaxLength(100);
                entity.Property(e => e.TransactionId).HasMaxLength(255);
                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderStatusHistory configuration
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Ticket configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IssueDescription).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Tickets)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TicketResponse configuration
            modelBuilder.Entity<TicketResponse>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Response).IsRequired();
                entity.Property(e => e.RespondedBy).HasMaxLength(255);
                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Responses)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
