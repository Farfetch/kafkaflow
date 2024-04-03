using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;

namespace KafkaFlow.Admin;

internal class TelemetryScheduler : ITelemetryScheduler
{
    private static readonly int s_processId = Process.GetCurrentProcess().Id;
    private readonly Dictionary<string, Timer> _timers = new();
    private readonly IDependencyResolver _dependencyResolver;
    private readonly ILogHandler _logHandler;

    public TelemetryScheduler(IDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
        _logHandler = dependencyResolver.Resolve<ILogHandler>();
    }

    public void Start(string telemetryId, string topicName, int topicPartition)
    {
        this.Stop(telemetryId);

        var consumers = _dependencyResolver
            .Resolve<IConsumerAccessor>()
            .All
            .Where(
                c => !c.ManagementDisabled &&
                     c.ClusterName.Equals(
                         _dependencyResolver
                             .Resolve<IConsumerAccessor>()[telemetryId]
                             .ClusterName))
            .ToList();

        var producer = _dependencyResolver.Resolve<IProducerAccessor>().GetProducer(telemetryId);

        _timers[telemetryId] = new Timer(
            _ => ProduceTelemetry(topicName, topicPartition, consumers, producer),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
    }

    public void Stop(string telemetryId)
    {
        if (_timers.TryGetValue(telemetryId, out var timer))
        {
            timer.Dispose();
            _timers.Remove(telemetryId);
        }
    }

    private void ProduceTelemetry(
        string topicName,
        int partition,
        IEnumerable<IMessageConsumer> consumers,
        IMessageProducer producer)
    {
        try
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
                                InstanceName = $"{Environment.MachineName}-{s_processId}",
                                PausedPartitions = c.PausedPartitions
                                    .Where(p => p.Topic == topic)
                                    .Select(p => p.Partition.Value),
                                RunningPartitions = c.RunningPartitions
                                    .Where(p => p.Topic == topic)
                                    .Select(p => p.Partition.Value),
                                WorkersCount = c.WorkersCount,
                                Status = c.Status,
                                Lag = consumerLag.Where(l => l.Topic == topic).Sum(l => l.Lag),
                                SentAt = DateTime.UtcNow,
                            });
                    });

            foreach (var item in items)
            {
                producer.Produce(topicName, Guid.NewGuid().ToByteArray(), item, partition: partition);
            }
        }
        catch (Exception e)
        {
            _logHandler.Warning("Error producing telemetry data", e, null);
        }
    }
}
