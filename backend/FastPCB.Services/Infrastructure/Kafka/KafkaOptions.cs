namespace FastPCB.Services.Infrastructure.Kafka
{
    public sealed class KafkaOptions
    {
        public bool Enabled { get; set; }
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string ClientId { get; set; } = "fastpcb-api";
        public KafkaTopicOptions Topics { get; set; } = new();
    }

    public sealed class KafkaTopicOptions
    {
        public string Projects { get; set; } = KafkaTopics.Projects;
        public string Reports { get; set; } = KafkaTopics.Reports;
        public string Users { get; set; } = KafkaTopics.Users;
    }

    public static class KafkaTopics
    {
        public const string Projects = "fastpcb.projects";
        public const string Reports = "fastpcb.reports";
        public const string Users = "fastpcb.users";
    }
}
