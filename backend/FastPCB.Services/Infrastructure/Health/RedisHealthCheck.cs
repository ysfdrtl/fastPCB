using FastPCB.Services.Infrastructure.Cache;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FastPCB.Services.Infrastructure.Health
{
    public sealed class RedisHealthCheck : IHealthCheck
    {
        private readonly RedisOptions _options;

        public RedisHealthCheck(IOptions<RedisOptions> options)
        {
            _options = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                return HealthCheckResult.Healthy("Redis is disabled.");
            }

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                return HealthCheckResult.Unhealthy("Redis is enabled but Redis:ConnectionString is empty.");
            }

            try
            {
                using var connection = await ConnectionMultiplexer.ConnectAsync(_options.ConnectionString);
                var database = connection.GetDatabase();
                var ping = await database.PingAsync();
                return HealthCheckResult.Healthy($"Redis ping succeeded in {ping.TotalMilliseconds:n0} ms.");
            }
            catch (Exception ex) when (ex is RedisException or ObjectDisposedException)
            {
                return HealthCheckResult.Unhealthy("Redis connection failed.", ex);
            }
        }
    }
}
