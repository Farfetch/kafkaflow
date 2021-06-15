namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class TelemetryScheduler : ITelemetryScheduler
    {
        private readonly Dictionary<string, Timer> timers = new();
        private readonly IDependencyResolver dependencyResolver;

        public TelemetryScheduler(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Start(string telemetryId, string topicName)
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
                _ => ProduceTelemetry(topicName, consumers, producer),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
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
            IReadOnlyCollection<IMessageConsumer> consumers,
            IMessageProducer producer)
        {
            var items = consumers.SelectMany(
                c => c.Assignment.Select(
                    a => new ConsumerMetric()
                    {
                        ConsumerName = c.ConsumerName,
                        Topic = a.Topic,
                        GroupId = c.GroupId,
                        InstanceName = $"{Environment.MachineName}-{c.MemberId}",
                        PausedPartitions = c.PausedPartitions
                            .Where(p => p.Topic == a.Topic)
                            .Select(p => p.Partition.Value),
                        RunningPartitions = c.RunningPartitions
                            .Where(p => p.Topic == a.Topic)
                            .Select(p => p.Partition.Value),
                        SentAt = DateTime.Now,
                    }));

            foreach (var item in items)
            {
                producer.Produce(topicName, Guid.NewGuid().ToByteArray(), item);
            }
        }
    }
}
