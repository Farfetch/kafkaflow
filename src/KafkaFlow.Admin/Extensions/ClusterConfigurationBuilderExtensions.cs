namespace KafkaFlow
{
    using System;
    using System.Linq;
    using System.Reflection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
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
            cluster.DependencyConfigurator.AddSingleton<IAdminProducer, AdminProducer>();

            cluster.DependencyConfigurator.AddSingleton<IMemoryCache, MemoryCache>();
            cluster.DependencyConfigurator.AddSingleton<ITelemetryCache, TelemetryCache>();

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
        /// <param name="topicName">The topic to be used by the metric commands</param>
        /// <param name="consumerGroup">The consumer group prefix</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string topicName,
            string consumerGroup)
        {
            cluster.DependencyConfigurator.AddSingleton<IMemoryCache, MemoryCache>();
            cluster.DependencyConfigurator.AddSingleton<ITelemetryCache, TelemetryCache>();

            var groupId =
                $"{consumerGroup}-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";

            var producerName = $"telemetry-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";

            return cluster
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .Topic(topicName)
                        .WithGroupId(groupId)
                        .WithWorkersCount(1)
                        .AsReadonly()
                        .WithBufferSize(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .WithPartitionsAssignedHandler((resolver, partitions) =>
                        {
                            TelemetryScheduler.Set(
                                groupId,
                                async _ =>
                                {
                                    await resolver
                                        .Resolve<IProducerAccessor>()
                                        .GetProducer(producerName)
                                        .BatchProduceAsync(
                                            resolver
                                                .Resolve<IConsumerAccessor>()
                                                .All
                                                .Where(c => !c.IsReadonly) // TODO GET ONLY CONSUMERS FROM THIS CLUSTER
                                                .SelectMany(c => c.Assignment.Select(a =>
                                                    new BatchProduceItem(
                                                        topicName,
                                                        Guid.NewGuid().ToString(),
                                                        new ConsumerMetric()
                                                        {
                                                            ConsumerName = c.ConsumerName,
                                                            Topic = a.Topic,
                                                            GroupId = c.GroupId,
                                                            HostName = $"{Environment.MachineName}-{c.MemberId}",
                                                            PausedPartitions = c.PausedPartitions
                                                                .Where(p => p.Topic == a.Topic)
                                                                .Select(p => p.Partition.Value),
                                                            RunningPartitions = c.RunningPartitions
                                                                .Where(p => p.Topic == a.Topic)
                                                                .Select(p => p.Partition.Value),
                                                            SentAt = DateTime.Now,
                                                        },
                                                        null)))
                                                .ToList());
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
                                        .AddHandlersFromAssemblyOf<ConsumerMetricHandler>())))
                .OnStop(_ => TelemetryScheduler.Unset(groupId));
        }

        /// <inheritdoc cref="EnableTelemetry(KafkaFlow.Configuration.IClusterConfigurationBuilder,string,string)"/>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string topicName)
        {
            return cluster.EnableTelemetry(topicName, $"Telemetry-{Assembly.GetEntryAssembly().GetName().Name}");
        }
    }
}
