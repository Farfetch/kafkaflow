namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using Configuration;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    
    public static class ClusterConfigurationBuilderExtensions
    {
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
