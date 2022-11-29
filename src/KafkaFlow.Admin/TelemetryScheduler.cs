namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class TelemetryScheduler : ITelemetryScheduler
    {
        private static readonly int ProcessId = Process.GetCurrentProcess().Id;
        private readonly Dictionary<string, Timer> timers = new();
        private readonly IDependencyResolver dependencyResolver;

        public TelemetryScheduler(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Start(string telemetryId, string topicName, int topicPartition)
        {
            this.Stop(telemetryId);

            var consumers = this.dependencyResolver
                .Resolve<IConsumerAccessor>()
                .All
                .Where(
                    c => !c.ManagementDisabled &&
                         c.ClusterName.Equals(
                             this.dependencyResolver
                                 .Resolve<IConsumerAccessor>()[telemetryId]
                                 .ClusterName))
                .ToList();

            var producer = this.dependencyResolver.Resolve<IProducerAccessor>().GetProducer(telemetryId);

            this.timers[telemetryId] = new Timer(
                _ => ProduceTelemetry(topicName, topicPartition, consumers, producer),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
        }

        public void Stop(string telemetryId)
        {
            if (this.timers.TryGetValue(telemetryId, out var timer))
            {
                timer.Dispose();
                this.timers.Remove(telemetryId);
            }
        }

        private static void ProduceTelemetry(
            string topicName,
            int partition,
            IEnumerable<IMessageConsumer> consumers,
            IMessageProducer producer)
        {
            var items = consumers
                .SelectMany(
                    c =>
                    {
                        var consumerLag = c.GetTopicPartitionsLag();

                        return c.Topics.Select(
                            topic => new ConsumerTelemetryMetric
                            {
                                ConsumerName = c.ConsumerName,
                                Topic = topic,
                                GroupId = c.GroupId,
                                InstanceName = $"{Environment.MachineName}-{ProcessId}",
                                PausedPartitions = c.PausedPartitions
                                    .Where(p => p.Topic == topic)
                                    .Select(p => p.Partition.Value),
                                RunningPartitions = c.RunningPartitions
                                    .Where(p => p.Topic == topic)
                                    .Select(p => p.Partition.Value),
                                WorkersCount = c.WorkersCount,
                                Status = c.Status,
                                Lag = consumerLag.Where(l => l.Topic == topic).Sum(l => l.Lag),
                                SentAt = DateTime.Now.ToUniversalTime(),
                            });
                    });

            foreach (var item in items)
            {
                producer.Produce(topicName, Guid.NewGuid().ToByteArray(), item, partition: partition);
            }
        }
    }
}
