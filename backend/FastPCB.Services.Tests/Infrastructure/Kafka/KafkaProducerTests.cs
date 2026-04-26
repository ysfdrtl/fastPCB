using FastPCB.Services.Infrastructure.Kafka;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace FastPCB.Services.Tests.Infrastructure.Kafka
{
    public sealed class KafkaProducerTests
    {
        [Fact]
        public async Task PublishAsync_WhenKafkaDisabled_DoesNotThrow()
        {
            var producer = new KafkaProducer(
                Options.Create(new KafkaOptions { Enabled = false }),
                NullLogger<KafkaProducer>.Instance);

            await producer.PublishAsync(
                KafkaTopics.Projects,
                "project-1",
                new ProjectCreated(1, 10, "Test project", DateTime.UtcNow));
        }

        [Fact]
        public async Task PublishAsync_WhenBootstrapServersMissing_DoesNotThrow()
        {
            var producer = new KafkaProducer(
                Options.Create(new KafkaOptions
                {
                    Enabled = true,
                    BootstrapServers = string.Empty
                }),
                NullLogger<KafkaProducer>.Instance);

            await producer.PublishAsync(
                KafkaTopics.Reports,
                "report-1",
                new ProjectReported(1, 2, 3, "Invalid content", DateTime.UtcNow));
        }
    }
}
