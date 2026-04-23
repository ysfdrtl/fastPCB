using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FastPCB.Data
{
    public class FastPCBContextFactory : IDesignTimeDbContextFactory<FastPCBContext>
    {
        // EF Core migration komutlari icin host bagimsiz bir DbContext uretir.
        public FastPCBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bulunamadi.");

            var optionsBuilder = new DbContextOptionsBuilder<FastPCBContext>();
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                builder => builder.MigrationsAssembly("FastPCB.Data"));

            return new FastPCBContext(optionsBuilder.Options);
        }
    }
}
