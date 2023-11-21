using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers
{
    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer _consumer;
        private readonly IDependencyResolver _resolver;

        private readonly ILogHandler _logHandler;

        private readonly object _commitSyncRoot = new();

        private Timer _commitTimer;
        private IReadOnlyList<Timer> _statisticsTimers;

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> _offsetsToCommit = new();

        public OffsetCommitter(
            IConsumer consumer,
            IDependencyResolver resolver,
            ILogHandler logHandler)
        {
            _consumer = consumer;
            _resolver = resolver;
            _logHandler = logHandler;
        }

        public List<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; } = new();

        public void MarkAsProcessed(TopicPartitionOffset tpo)
        {
            _offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition),
                tpo,
                (_, _) => tpo);
        }

        public Task StartAsync()
        {
            _commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                _consumer.Configuration.AutoCommitInterval,
                _consumer.Configuration.AutoCommitInterval);

            _statisticsTimers = this.PendingOffsetsStatisticsHandlers
                .Select(
                    handler => new Timer(
                        _ => this.PendingOffsetsHandler(handler),
                        null,
                        handler.Interval,
                        handler.Interval))
                .ToList();

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _commitTimer.Dispose();
            this.CommitHandler();

            foreach (var timer in _statisticsTimers)
            {
                timer.Dispose();
            }

            return Task.CompletedTask;
        }

        private void PendingOffsetsHandler(PendingOffsetsStatisticsHandler handler)
        {
            if (!_offsetsToCommit.IsEmpty)
            {
                handler.Handler(
                    _resolver,
                    _offsetsToCommit.Values.Select(
                        x =>
                            new Confluent.Kafka.TopicPartitionOffset(x.Topic, x.Partition, x.Offset)));
            }
        }

        private void CommitHandler()
        {
            lock (_commitSyncRoot)
            {
                ConcurrentDictionary<(string, int), TopicPartitionOffset> offsets = null;

                try
                {
                    if (!_offsetsToCommit.Any())
                    {
                        return;
                    }

                    offsets = Interlocked.Exchange(
                        ref _offsetsToCommit,
                        new ConcurrentDictionary<(string, int), TopicPartitionOffset>());

                    _consumer.Commit(
                        offsets.Values
                            .Select(x => new Confluent.Kafka.TopicPartitionOffset(x.Topic, x.Partition, x.Offset + 1))
                            .ToList());

                    if (!_consumer.Configuration.ManagementDisabled)
                    {
                        this.LogOffsetsCommitted(offsets.Values);
                    }
                }
                catch (Exception e)
                {
                    _logHandler.Warning(
                        "Error Commiting Offsets",
                        new { ErrorMessage = e.Message });

                    if (offsets is not null)
                    {
                        this.RequeueFailedOffsets(offsets.Values);
                    }
                }
            }
        }

        private void LogOffsetsCommitted(IEnumerable<TopicPartitionOffset> offsets)
        {
            _logHandler.Verbose(
                "Offsets committed",
                new
                {
                    Offsets = offsets.GroupBy(
                        x => x.Topic,
                        (topic, groupedOffsets) => new
                        {
                            Topic = topic,
                            Partitions = groupedOffsets.Select(
                                offset => new
                                {
                                    offset.Partition,
                                    offset.Offset,
                                }),
                        }),
                });
        }

        private void RequeueFailedOffsets(IEnumerable<TopicPartitionOffset> offsets)
        {
            foreach (var tpo in offsets)
            {
                _offsetsToCommit.TryAdd((tpo.Topic, tpo.Partition), tpo);
            }
        }
    }
}
