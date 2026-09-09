using System;
using System.Net;
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
        cluster.DependencyConfigurator.AddSingleton<ISchemaRegistryClient>(_ => new CachedSchemaRegistryClient(config));
        return cluster;
    }

    /// <summary>
    /// Configures schema registry to the cluster with a custom <see cref="IAuthenticationHeaderValueProvider"/> and optional <see cref="IWebProxy"/>
    /// </summary>
    /// <param name="cluster">Instance of <see cref="IClusterConfigurationBuilder"/></param>
    /// <param name="handler">A handler to set the configuration values</param>
    /// <param name="authenticationHeaderValueProvider">A custom authentication header value provider</param>
    /// <param name="webProxy">A proxy server to use for connections</param>
    /// <returns></returns>
    public static IClusterConfigurationBuilder WithSchemaRegistry(
        this IClusterConfigurationBuilder cluster,
        Action<SchemaRegistryConfig> handler,
        IAuthenticationHeaderValueProvider authenticationHeaderValueProvider,
        IWebProxy webProxy = null)
    {
        var config = new SchemaRegistryConfig();
        handler(config);
        cluster.DependencyConfigurator.AddSingleton<ISchemaRegistryClient>(_ => new CachedSchemaRegistryClient(config, authenticationHeaderValueProvider, webProxy));
        return cluster;
    }
}
