using FastPCB.Data.Configuration;
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
            EnvFileLoader.LoadNearest(basePath);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = MySqlConnectionStringResolver.Resolve(configuration);

            var optionsBuilder = new DbContextOptionsBuilder<FastPCBContext>();
            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0)),
                builder =>
                {
                    builder.MigrationsAssembly("FastPCB.Data");
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

            return new FastPCBContext(optionsBuilder.Options);
        }
    }
}
