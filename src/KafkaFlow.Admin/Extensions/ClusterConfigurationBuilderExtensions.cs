namespace KafkaFlow
{
    using System;
    using System.Linq;
    using System.Reflection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.Producers;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Creates the admin producer and consumer to manage the application consumers
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="adminTopic">The topic to be used by the admin commands</param>
        /// <param name="adminConsumerGroup">The consumer group prefix</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string adminTopic,
            string adminConsumerGroup)
        {
            cluster.DependencyConfigurator.AddSingleton<IMemoryCache, MemoryCache>();

            cluster.DependencyConfigurator.AddSingleton<IAdminProducer, AdminProducer>();

            return cluster
                .AddProducer<AdminProducer>(
                    producer => producer
                        .DefaultTopic(adminTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .Topic(adminTopic)
                        .WithGroupId(
                            $"{adminConsumerGroup}-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}")
                        .WithWorkersCount(1)
                        .WithBufferSize(1)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AsReadonly()
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlersFromAssemblyOf<ResetConsumerOffsetHandler>())));
        }

        /// <inheritdoc cref="EnableAdminMessages(KafkaFlow.Configuration.IClusterConfigurationBuilder,string,string)"/>
        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string adminTopic)
        {
            return cluster.EnableAdminMessages(adminTopic, $"Admin-{Assembly.GetEntryAssembly().GetName().Name}");
        }

        /// <summary>
        /// Creates the telemetry producer and consumer to send and receive metric messages
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="metricTopic">The topic to be used by the metric commands</param>
        /// <param name="metricConsumerGroup">The consumer group prefix</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string metricTopic,
            string metricConsumerGroup)
        {
            cluster.DependencyConfigurator.AddSingleton<IMemoryCache, MemoryCache>();

            cluster.DependencyConfigurator.AddSingleton<ITelemetryProducer, TelemetryProducer>();

            return cluster
                .AddProducer<TelemetryProducer>(
                    producer => producer
                        .DefaultTopic(metricTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .Topic(metricTopic)
                        .WithGroupId(
                            $"{metricConsumerGroup}-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}")
                        .WithWorkersCount(1)
                        .AsReadonly()
                        .WithBufferSize(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .WithPartitionsAssignedHandler((resolver, partitions) =>
                        {
                            TimerManager.Instance.Set(
                                async _ =>
                                {
                                    foreach (var c in resolver.Resolve<IConsumerAccessor>().All.Where(c => !c.IsReadonly))
                                    {
                                        foreach (var assignment in c.Assignment )
                                        {
                                            await resolver.Resolve<ITelemetryProducer>().ProduceAsync(
                                                new ConsumerMetric()
                                                {
                                                    ConsumerName = c.ConsumerName,
                                                    Topic = assignment.Topic,
                                                    GroupId = c.GroupId,
                                                    HostName = $"{Environment.MachineName}-{c.MemberId}",
                                                    PausedPartitions = c.PausedPartitions
                                                        .Where(p=> p.Topic == assignment.Topic)
                                                        .Select(p => p.Partition.Value),
                                                    RunningPartitions = c.RunningPartitions
                                                        .Where(p=> p.Topic == assignment.Topic)
                                                        .Select(p => p.Partition.Value),
                                                    SentAt = DateTime.Now,
                                                });
                                        }
                                    }
                                },
                                TimeSpan.Zero,
                                TimeSpan.FromSeconds(1));
                        })
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlersFromAssemblyOf<ConsumerMetricHandler>())));
        }

        /// <inheritdoc cref="EnableTelemetry(KafkaFlow.Configuration.IClusterConfigurationBuilder,string,string)"/>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string adminTopic)
        {
            return cluster.EnableTelemetry(adminTopic, $"Telemetry-{Assembly.GetEntryAssembly().GetName().Name}");
        }
    }
}
