using FastPCB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastPCB.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar(50)");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("int");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.FilePath)
                .HasMaxLength(500)
                .HasColumnType("varchar(500)");

            builder.Property(p => p.Description)
                .HasColumnType("text");

            builder.Property(p => p.Layers)
                .HasColumnType("int");

            builder.Property(p => p.Material)
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.Property(p => p.MinDistance)
                .HasColumnType("double");

            builder.Property(p => p.Quantity)
                .HasColumnType("int");

            builder.HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Projects_Users_UserId");

            builder.HasIndex(p => p.UserId)
                .HasDatabaseName("IX_Projects_UserId");

            builder.HasIndex(p => p.Title)
                .HasDatabaseName("IX_Projects_Title");

            builder.ToTable("Projects");
        }
    }
}
