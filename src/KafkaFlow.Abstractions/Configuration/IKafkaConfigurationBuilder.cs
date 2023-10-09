namespace KafkaFlow.Configuration;

using System;

/// <summary>
/// A builder to configure KafkaFlow
/// </summary>
public interface IKafkaConfigurationBuilder
{
    /// <summary>
    /// Adds a new Cluster
    /// </summary>
    /// <param name="cluster">A handle to configure the cluster</param>
    /// <returns></returns>
    IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster);

    /// <summary>
    /// Set the log handler to be used by the Framework, if none is provided the <see cref="NullLogHandler"/> will be used
    /// </summary>
    /// <typeparam name="TLogHandler">A class that implements the <see cref="ILogHandler"/> interface</typeparam>
    /// <returns></returns>
    IKafkaConfigurationBuilder UseLogHandler<TLogHandler>()
        where TLogHandler : ILogHandler;

    /// <summary>
    /// Subscribe the global events defined in <see cref="IGlobalEvents"/>
    /// </summary>
    /// <param name="observers">A handle to subscribe the events</param>
    /// <returns></returns>
    IKafkaConfigurationBuilder SubscribeGlobalEvents(Action<IGlobalEvents> observers);
}
