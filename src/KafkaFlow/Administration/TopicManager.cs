namespace KafkaFlow.Administration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using KafkaFlow.Configuration;

    internal class TopicManager : ITopicManager
    {
        private readonly ILogHandler logHandler;

        public TopicManager(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
        }

        public async Task CreateIfNotExistsAsync(string servers, IEnumerable<TopicConfiguration> configurations)
        {
            using var adminClient =
                new AdminClientBuilder(new AdminClientConfig { BootstrapServers = servers }).Build();

            try
            {
                var topics = configurations.Select(
                    configuration => new TopicSpecification
                    {
                        Name = configuration.Name, ReplicationFactor = configuration.ReplicationFactor,
                        NumPartitions = configuration.NumberOfPartitions,
                    }).ToArray();

                await adminClient.CreateTopicsAsync(
                    topics);
            }
            catch (CreateTopicsException exception)
            {
                var hasNonExpectedErrors = false;
                foreach (var exceptionResult in exception.Results)
                {
                    if (exceptionResult.Error.Code == ErrorCode.TopicAlreadyExists)
                    {
                        this.logHandler.Warning(
                            "An error occurred creating topic {Topic}: {Reason}",
                            new
                            {
                                exceptionResult.Topic, exceptionResult.Error.Reason,
                            });
                        continue;
                    }

                    hasNonExpectedErrors = true;
                }

                if (hasNonExpectedErrors)
                {
                    this.logHandler.Error(
                        "An error occurred creating topics",
                        exception,
                        new
                        {
                            Servers = servers,
                        });
                    throw;
                }
            }
        }
    }
}
