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

        public void Start(string key, string topicName)
        {
            this.Stop(key);

            var consumers = this.dependencyResolver
                .Resolve<IConsumerAccessor>()
                .All
                .Where(
                    c => !c.ManagementDisabled &&
                         c.ClusterName.Equals(
                             this.dependencyResolver
                                 .Resolve<IConsumerAccessor>()[key]
                                 .ClusterName))
                .ToList();

            var producer = this.dependencyResolver.Resolve<IProducerAccessor>().GetProducer(key);

            this.timers[key] = new Timer(
                _ => ProduceTelemetry(topicName, consumers, producer),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
        }

        public void Stop(string key)
        {
            if (this.timers.TryGetValue(key, out var timer))
            {
                timer.Dispose();
                this.timers.Remove(key);
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
