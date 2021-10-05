namespace KafkaFlow.Extensions.ProducerThrottling.Extensions
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Configuration;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configures the lag reader kafkaflow client to the cluster
        /// </summary>
        /// <param name="cluster">Instance of <see cref="IClusterConfigurationBuilder"/></param>
        /// <param name="brokers">The brokers address to be used</param>
        /// <param name="clientId">The client id to be used</param>
        /// <param name="timeout">The interval of time to wait for a operation to be completed. The default value is 5 seconds.</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder WithLagReader(
            this IClusterConfigurationBuilder cluster,
            IEnumerable<BrokerAddress> brokers,
            string clientId = null,
            TimeSpan? timeout = null)
        {
            cluster.DependencyConfigurator.AddSingleton(_ => MetricsReaderBuilder.BuildLagReader(brokers, clientId, timeout));
            return cluster;
        }
    }
}
