namespace KafkaFlow.Configuration
{
    using System;

    /// <summary>
    /// </summary>
    public interface IKafkaConfigurationBuilder
    {
        /// <summary>
        /// Adds a new Cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster);

        /// <summary>
        /// Set the log handler to be used by the Framework, if none is provided the <see cref="NullLogHandler"/> will be used
        /// </summary>
        /// <typeparam name="TLogHandler">A class that implements the <see cref="ILogHandler"/> interface</typeparam>
        /// <returns></returns>
        IKafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler;
    }
}
