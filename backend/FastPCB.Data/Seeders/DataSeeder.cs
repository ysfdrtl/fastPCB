using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Data.Seeders
{
    public static class DataSeeder
    {
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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
