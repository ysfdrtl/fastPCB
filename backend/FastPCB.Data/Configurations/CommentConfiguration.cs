using FastPCB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastPCB.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ProjectId)
                .IsRequired();

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnType("varchar(1000)");

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(c => c.Project)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_Projects_ProjectId");

            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_Users_UserId");

            builder.HasIndex(c => c.ProjectId)
                .HasDatabaseName("IX_Comments_ProjectId");

            builder.HasIndex(c => c.UserId)
                .HasDatabaseName("IX_Comments_UserId");

            builder.ToTable("Comments");
        }
    }
}
