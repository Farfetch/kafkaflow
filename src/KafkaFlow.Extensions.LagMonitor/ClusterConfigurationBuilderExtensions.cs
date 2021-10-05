#nullable enable
namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using System.Collections.Generic;
    using Client.Producers;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    public static class ClusterConfigurationBuilderExtensions
    {
        public static IClusterConfigurationBuilder WithLagReader(
            this IClusterConfigurationBuilder cluster,
            IEnumerable<BrokerAddress> brokers,
            string? clientId = null,
            TimeSpan? timeout = null)
        {
            cluster.DependencyConfigurator.AddSingleton<ILagReader>(_ => MetricsReaderBuilder.BuildLagReader(brokers, clientId, timeout));
            return cluster;
        }
    }
}
