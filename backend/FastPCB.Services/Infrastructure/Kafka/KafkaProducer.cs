using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastPCB.Services.Infrastructure.Kafka
{
    public sealed class KafkaProducer : IKafkaProducer, IDisposable
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly KafkaOptions _options;
        private readonly ILogger<KafkaProducer> _logger;
        private readonly Lazy<IProducer<string, string>?> _producer;

        public KafkaProducer(IOptions<KafkaOptions> options, ILogger<KafkaProducer> logger)
        {
            _options = options.Value;
            _logger = logger;
            _producer = new Lazy<IProducer<string, string>?>(CreateProducer);
        }

        public async Task PublishAsync<TEvent>(string topic, string key, TEvent payload, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                return;
            }

            var producer = _producer.Value;
            if (producer is null)
            {
                return;
            }

            try
            {
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = JsonSerializer.Serialize(payload, SerializerOptions)
                };

                await producer.ProduceAsync(topic, message, cancellationToken);
            }
            catch (Exception ex) when (ex is ProduceException<string, string> or KafkaException or OperationCanceledException)
            {
                // Kafka must never make the primary business transaction fail.
                _logger.LogWarning(ex, "Kafka publish failed for topic {Topic} and key {Key}.", topic, key);
            }
        }

        public void Dispose()
        {
            if (_producer.IsValueCreated)
            {
                _producer.Value?.Dispose();
            }
        }

        private IProducer<string, string>? CreateProducer()
        {
            if (string.IsNullOrWhiteSpace(_options.BootstrapServers))
            {
                _logger.LogWarning("Kafka is enabled but Kafka:BootstrapServers is empty.");
                return null;
            }

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = _options.BootstrapServers,
                    ClientId = _options.ClientId,
                    Acks = Acks.Leader,
                    EnableIdempotence = false,
                    MessageTimeoutMs = 5000
                };

                return new ProducerBuilder<string, string>(config).Build();
            }
            catch (Exception ex) when (ex is KafkaException or ArgumentException)
            {
                _logger.LogWarning(ex, "Kafka producer could not be created.");
                return null;
            }
        }
    }
}
