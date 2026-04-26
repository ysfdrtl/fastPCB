namespace FastPCB.Services.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    }
}
