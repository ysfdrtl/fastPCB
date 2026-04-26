namespace FastPCB.Services.Infrastructure.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync<TEvent>(string topic, string key, TEvent payload, CancellationToken cancellationToken = default);
    }
}
