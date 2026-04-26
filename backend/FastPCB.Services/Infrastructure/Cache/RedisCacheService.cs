using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FastPCB.Services.Infrastructure.Cache
{
    public sealed class RedisCacheService : ICacheService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private readonly RedisOptions _options;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private IConnectionMultiplexer? _connection;

        public RedisCacheService(IOptions<RedisOptions> options, ILogger<RedisCacheService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            CancellationToken cancellationToken = default)
        {
            var database = await TryGetDatabaseAsync();
            if (database is null)
            {
                return await factory();
            }

            var cacheKey = BuildKey(key);

            try
            {
                var cachedValue = await database.StringGetAsync(cacheKey);
                if (cachedValue.HasValue)
                {
                    var cached = JsonSerializer.Deserialize<T>(cachedValue!, SerializerOptions);
                    if (cached is not null)
                    {
                        return cached;
                    }
                }

                var value = await factory();
                var ttl = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(Math.Max(1, _options.DefaultExpirationMinutes));
                await database.StringSetAsync(cacheKey, JsonSerializer.Serialize(value, SerializerOptions), ttl);
                await IndexKeyAsync(database, key);

                return value;
            }
            catch (Exception ex) when (ex is RedisException or ObjectDisposedException or JsonException)
            {
                _logger.LogWarning(ex, "Redis cache failed for key {CacheKey}. Falling back to source.", key);
                return await factory();
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var database = await TryGetDatabaseAsync();
            if (database is null)
            {
                return;
            }

            try
            {
                await database.KeyDeleteAsync(BuildKey(key));
            }
            catch (Exception ex) when (ex is RedisException or ObjectDisposedException)
            {
                _logger.LogWarning(ex, "Redis cache remove failed for key {CacheKey}.", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            var database = await TryGetDatabaseAsync();
            if (database is null)
            {
                return;
            }

            try
            {
                var indexKey = BuildIndexKey(prefix);
                var keys = await database.SetMembersAsync(indexKey);
                if (keys.Length > 0)
                {
                    await database.KeyDeleteAsync(keys.Select(value => (RedisKey)value.ToString()).ToArray());
                }

                await database.KeyDeleteAsync(indexKey);
            }
            catch (Exception ex) when (ex is RedisException or ObjectDisposedException)
            {
                _logger.LogWarning(ex, "Redis cache prefix remove failed for prefix {CachePrefix}.", prefix);
            }
        }

        private async Task<IDatabase?> TryGetDatabaseAsync()
        {
            if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                return null;
            }

            try
            {
                if (_connection?.IsConnected == true)
                {
                    return _connection.GetDatabase();
                }

                await _connectionLock.WaitAsync();
                try
                {
                    if (_connection?.IsConnected != true)
                    {
                        _connection?.Dispose();
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConnectionString);
                    }
                }
                finally
                {
                    _connectionLock.Release();
                }

                return _connection.GetDatabase();
            }
            catch (Exception ex) when (ex is RedisException or ObjectDisposedException)
            {
                _logger.LogWarning(ex, "Redis connection unavailable. Continuing without cache.");
                return null;
            }
        }

        private async Task IndexKeyAsync(IDatabase database, string key)
        {
            var prefixEnd = key.IndexOf(':');
            while (prefixEnd > 0)
            {
                var prefix = key[..prefixEnd];
                await database.SetAddAsync(BuildIndexKey(prefix), BuildKey(key));
                prefixEnd = key.IndexOf(':', prefixEnd + 1);
            }
        }

        private string BuildKey(string key) => $"{_options.InstanceName}{key}";
        private string BuildIndexKey(string prefix) => $"{_options.InstanceName}cache-index:{prefix}";
    }
}
