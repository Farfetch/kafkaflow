namespace KafkaFlow
{
    using System;
    using Confluent.SchemaRegistry;
    using KafkaFlow.Configuration;

    /// <summary>
    /// 
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configures schema registry to the cluster
        /// </summary>
        /// <param name="cluster">Instance of <see cref="IClusterConfigurationBuilder"/></param>
        /// <param name="handler">A handler to set the configuration values</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder WithSchemaRegistry(
            this IClusterConfigurationBuilder cluster,
            Action<SchemaRegistryConfig> handler)
        {
            var config = new SchemaRegistryConfig();
            handler(config);
            cluster.DependencyConfigurator.AddTransient<ISchemaRegistryClient>(_ => new CachedSchemaRegistryClient(config));
            return cluster;
        }
    }
}
