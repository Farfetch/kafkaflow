using System;
using Confluent.SchemaRegistry;
using KafkaFlow.Configuration;

namespace KafkaFlow;

/// <summary>
/// No needed
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

        cluster.DependencyConfigurator
            .AddSingleton<ISchemaRegistryClientFactory, SchemaRegistryClientFactory>()
            .AddSingleton(resolver => resolver.Resolve<ISchemaRegistryClientFactory>().CreateSchemaRegistryClient(config));

        return cluster;
    }
}
