using FastPCB.Data.Configurations;
using FastPCB.Data.Seeders;
using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Data
{
    public class FastPCBContext : DbContext
    {
        // EF Core context nesnesini verilen baglanti ayarlariyla baslatir.
        public FastPCBContext(DbContextOptions<FastPCBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ProjectLike> ProjectLikes { get; set; }
        public DbSet<Ticket> Reports { get; set; }

        // Tum entity konfigurasyonlarini ve seed verilerini modele uygular.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectLikeConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());

            // Seed initial data
            DataSeeder.SeedData(modelBuilder);
        }
    }
}
