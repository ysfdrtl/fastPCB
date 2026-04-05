using FastPCB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastPCB.Data.Configurations
{
    public class ProjectLikeConfiguration : IEntityTypeConfiguration<ProjectLike>
    {
        public void Configure(EntityTypeBuilder<ProjectLike> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.ProjectId)
                .IsRequired();

            builder.Property(l => l.UserId)
                .IsRequired();

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(l => l.Project)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProjectLikes_Projects_ProjectId");

            builder.HasOne(l => l.User)
                .WithMany(u => u.LikedProjects)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProjectLikes_Users_UserId");

            builder.HasIndex(l => l.ProjectId)
                .HasDatabaseName("IX_ProjectLikes_ProjectId");

            builder.HasIndex(l => l.UserId)
                .HasDatabaseName("IX_ProjectLikes_UserId");

            builder.HasIndex(l => new { l.ProjectId, l.UserId })
                .IsUnique()
                .HasDatabaseName("IX_ProjectLikes_ProjectId_UserId");

            builder.ToTable("ProjectLikes");
        }
    }
}
