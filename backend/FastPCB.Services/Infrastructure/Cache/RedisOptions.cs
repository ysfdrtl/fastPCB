namespace FastPCB.Services.Infrastructure.Cache
{
    public sealed class RedisOptions
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; } = "localhost:6379";
        public string InstanceName { get; set; } = "fastpcb:";
        public int DefaultExpirationMinutes { get; set; } = 5;
    }
}
