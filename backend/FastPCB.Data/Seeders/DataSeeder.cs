using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Data.Seeders
{
    public static class DataSeeder
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 4, 23, 22, 20, 37, 869, DateTimeKind.Utc);
        private static readonly DateTime SeedUpdatedAt = new(2026, 4, 23, 22, 20, 37, 869, DateTimeKind.Utc);

        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Test user data (sifre: Test1234!)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "test@fastpcb.com",
                    PasswordHash = "100000.Tba38WWIwK53hEPbMlTckA==.YD7TDB6tiwSXaLZHp9kT52il8qibRa33aNpJ9G2sqPw=",
                    FirstName = "Test",
                    LastName = "User",
                    Phone = "+905551234567",
                    Address = "Isparta, Turkey",
                    Role = UserRole.User,
                    CreatedAt = SeedCreatedAt.AddTicks(4290),
                    UpdatedAt = SeedUpdatedAt.AddTicks(4291)
                },
                new User
                {
                    Id = 2,
                    Email = "admin@fastpcb.com",
                    PasswordHash = "100000.AQIDBAUGBwgJCgsMDQ4PEA==.FMliGP5Jcjft/GMzn7MtR2CZciACjUsOHdwvp/0h/cs=",
                    FirstName = "Admin",
                    LastName = "User",
                    Phone = "+905551111111",
                    Address = "Isparta, Turkey",
                    Role = UserRole.Admin,
                    CreatedAt = SeedCreatedAt.AddTicks(4292),
                    UpdatedAt = SeedUpdatedAt.AddTicks(4293)
                }
            );
        }
    }
}
