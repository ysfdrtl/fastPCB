using FastPCB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastPCB.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.ProjectId)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(t => t.Description)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("int");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(t => t.Response)
                .HasColumnType("text");

            // Foreign key configuration
            builder.HasOne(t => t.Project)
                .WithMany(p => p.Reports)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reports_Projects_ProjectId");

            builder.HasOne(t => t.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reports_Users_UserId");

            // Index configuration
            builder.HasIndex(t => t.ProjectId)
                .HasDatabaseName("IX_Reports_ProjectId");

            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_Reports_UserId");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Reports_Status");

            builder.HasIndex(t => new { t.ProjectId, t.UserId })
                .HasDatabaseName("IX_Reports_ProjectId_UserId");

            // Table configuration
            builder.ToTable("Reports");
        }
    }
}
