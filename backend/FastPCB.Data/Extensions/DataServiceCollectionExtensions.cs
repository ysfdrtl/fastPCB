using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastPCB.Data.Extensions
{
    public static class DataServiceCollectionExtensions
    {
        /// <summary>
        /// Veritabanı bağlamını ve gerekli servisleri DI container'a ekler.
        /// </summary>
        public static IServiceCollection AddFastPCBData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FastPCBContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    builder => builder.MigrationsAssembly("FastPCB.Data")
                )
            );

            return services;
        }

        /// <summary>
        /// Migration'ı otomatik olarak çalıştırır.
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FastPCBContext>();
                
                // Migration'ları uygula
                await dbContext.Database.MigrateAsync();
            }
        }

        /// <summary>
        /// Veritabanını başlatır ve seed data'sı ekler.
        /// </summary>
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FastPCBContext>();

                // Migration'ları uygula
                await dbContext.Database.MigrateAsync();

                // Ensure database is created
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }
}
