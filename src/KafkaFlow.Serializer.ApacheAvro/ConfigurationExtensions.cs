namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using Configuration;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    
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
            cluster.DependencyConfigurator.AddTransient<ISchemaRegistryClient>(factory => new CachedSchemaRegistryClient(config));
            return cluster;
        }
    }
}
