using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastPCB.Data.Extensions
{
    public static class DataServiceCollectionExtensions
    {
        /// <summary>
        /// Veritabani baglamini ve gerekli servisleri DI container'a ekler.
        /// </summary>
        public static IServiceCollection AddFastPCBData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FastPCBContext>(options =>
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 0)),
                    builder => builder
                        .MigrationsAssembly("FastPCB.Data")
                        .EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelaySeconds: 30,
                            errorNumbersToAdd: null
                        )
                )
            );

            return services;
        }

        /// <summary>
        /// Migration'i otomatik olarak calistirir.
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FastPCBContext>();

            await dbContext.Database.MigrateAsync();
        }

        /// <summary>
        /// Veritabanini baslatir ve seed data'si ekler.
        /// </summary>
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FastPCBContext>();

            await dbContext.Database.MigrateAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
