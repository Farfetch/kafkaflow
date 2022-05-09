namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Provides extension methods over <see cref="IConsumerConfigurationBuilder"/> and <see cref="IProducerConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Sets configurations in the producer based on a <see cref="P:Confluent.Kafka.ProducerConfig"/> instance
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IProducerConfigurationBuilder"/></param>
        /// <param name="config"><see cref="P:Confluent.Kafka.ProducerConfig"/> instance</param>
        /// <returns></returns>
        public static IProducerConfigurationBuilder WithProducerConfig(this IProducerConfigurationBuilder builder, ProducerConfig config)
        {
            return ((ProducerConfigurationBuilder) builder).WithProducerConfig(config);
        }

        /// <summary>
        /// Sets compression configurations in the producer
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IProducerConfigurationBuilder"/></param>
        /// <param name="compressionType">
        /// <see cref="P:Confluent.Kafka.CompressionType"/> enum to select the compression codec to use for compressing message sets. This is the default value for all topics, may be overridden by the topic configuration property `compression.codec`.
        /// default: none
        /// importance: medium</param>
        /// <param name="compressionLevel">
        /// Compression level parameter for algorithm selected by <see cref="P:Confluent.Kafka.CompressionType"/> enum. Higher values will result in better compression at the cost of more CPU usage. Usable range is algorithm-dependent: [0-9] for gzip; [0-12] for lz4; only 0 for snappy; -1 = codec-dependent default compression level.
        /// default: -1
        /// importance: medium
        /// </param>
        /// <returns></returns>
        public static IProducerConfigurationBuilder WithCompression(
            this IProducerConfigurationBuilder builder,
            CompressionType compressionType,
            int? compressionLevel = -1)
        {
            return ((ProducerConfigurationBuilder) builder).WithCompression(compressionType, compressionLevel);
        }

        /// <summary>
        /// Sets configurations in the consumer based on a <see cref="P:Confluent.Kafka.ConsumerConfig"/> instance
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IConsumerConfigurationBuilder"/></param>
        /// <param name="config"><see cref="P:Confluent.Kafka.ConsumerConfig"/> instance</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithConsumerConfig(this IConsumerConfigurationBuilder builder, ConsumerConfig config)
        {
            return ((ConsumerConfigurationBuilder) builder).WithConsumerConfig(config);
        }

        /// <summary>
        /// Adds a handler for the Kafka consumer partitions assigned
        /// </summary>
        /// <param name="builder">The configuration builder</param>
        /// <param name="partitionsAssignedHandler">A handler for the consumer partitions assigned</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithPartitionsAssignedHandler(
            this IConsumerConfigurationBuilder builder,
            Action<IDependencyResolver, List<TopicPartition>> partitionsAssignedHandler)
        {
            return ((ConsumerConfigurationBuilder)builder).WithPartitionsAssignedHandler(partitionsAssignedHandler);
        }

        /// <summary>
        /// Adds a handler for the Kafka consumer partitions revoked
        /// </summary>
        /// <param name="builder">The configuration builder</param>
        /// <param name="partitionsRevokedHandler">A handler for the consumer partitions revoked</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithPartitionsRevokedHandler(
            this IConsumerConfigurationBuilder builder,
            Action<IDependencyResolver, List<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            return ((ConsumerConfigurationBuilder)builder).WithPartitionsRevokedHandler(partitionsRevokedHandler);
        }

        /// <summary>
        /// Adds a handler to be executed when KafkaFlow has pending offsets to be committed
        /// </summary>
        /// <param name="builder">The configuration builder</param>
        /// <param name="pendingOffsetsHandler">A handler for the consumer pending offsets state</param>
        /// <param name="interval">The time interval between handler executions</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithPendingOffsetsStatisticsHandler(
            this IConsumerConfigurationBuilder builder,
            Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> pendingOffsetsHandler,
            TimeSpan interval)
        {
            return ((ConsumerConfigurationBuilder)builder).WithPendingOffsetsStatisticsHandler(pendingOffsetsHandler, interval);
        }

        /// <summary>
        /// Register a custom consumer factory to be internally used by the framework
        /// </summary>
        /// <param name="builder">The configuration builder</param>
        /// <param name="decoratorFactory">The factory method</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithCustomFactory(
            this IConsumerConfigurationBuilder builder,
            ConsumerCustomFactory decoratorFactory)
        {
            return ((ConsumerConfigurationBuilder) builder).WithCustomFactory(decoratorFactory);
        }

        /// <summary>
        /// Register a custom producer factory to be internally used by the framework
        /// </summary>
        /// <param name="builder">The configuration builder</param>
        /// <param name="decoratorFactory">The factory method</param>
        /// <returns></returns>
        public static IProducerConfigurationBuilder WithCustomFactory(
            this IProducerConfigurationBuilder builder,
            ProducerCustomFactory decoratorFactory)
        {
            return ((ProducerConfigurationBuilder) builder).WithCustomFactory(decoratorFactory);
        }
    }
}
