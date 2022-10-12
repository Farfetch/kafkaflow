namespace KafkaFlow.Clusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using KafkaFlow.Configuration;

    internal class ClusterManager : IClusterManager, IDisposable
    {
        private readonly ILogHandler logHandler;
        private readonly Lazy<IAdminClient> lazyAdminClient;
        private readonly ClusterConfiguration configuration;

        public ClusterManager(ILogHandler logHandler, ClusterConfiguration configuration)
        {
            this.logHandler = logHandler;
            this.configuration = configuration;
            this.lazyAdminClient = new Lazy<IAdminClient>(
                () => new AdminClientBuilder(new AdminClientConfig
                    { BootstrapServers = string.Join(",", configuration.Brokers) }).Build());
        }

        public string ClusterName => this.configuration.Name;

        public async Task CreateIfNotExistsAsync(IEnumerable<TopicConfiguration> configurations)
        {
            try
            {
                var topics = configurations.Select(
                    topicConfiguration => new TopicSpecification
                    {
                        Name = topicConfiguration.Name,
                        ReplicationFactor = topicConfiguration.ReplicationFactor,
                        NumPartitions = topicConfiguration.NumberOfPartitions,
                    }).ToArray();

                await this.lazyAdminClient.Value.CreateTopicsAsync(
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
                            Servers = this.configuration.Brokers,
                        });
                    throw;
                }
            }
        }

        public void Dispose()
            => this.lazyAdminClient.Value.Dispose();
    }
}
