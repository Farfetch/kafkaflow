namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer consumer;
        private readonly ILogHandler logHandler;

        private readonly Timer commitTimer;
        private readonly IReadOnlyList<Timer> statisticsTimers;

        private readonly object commitSyncRoot = new();

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> offsetsToCommit = new();

        public OffsetCommitter(
            IConsumer consumer,
            IDependencyResolver resolver,
            IReadOnlyList<(Action<IDependencyResolver, IEnumerable<Confluent.Kafka.TopicPartitionOffset>> handler, TimeSpan interval)>
                pendingOffsetsHandlers,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.logHandler = logHandler;

            this.commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                consumer.Configuration.AutoCommitInterval,
                consumer.Configuration.AutoCommitInterval);

            this.statisticsTimers = pendingOffsetsHandlers
                .Select(
                    s => new Timer(
                        _ => this.PendingOffsetsHandler(resolver, s.handler),
                        null,
                        TimeSpan.Zero,
                        s.interval))
                .ToList();
        }

        public void Dispose()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();

            foreach (var timer in this.statisticsTimers)
            {
                timer.Dispose();
            }
        }

        public void MarkAsProcessed(TopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition),
                tpo,
                (_, _) => tpo);
        }

        private void PendingOffsetsHandler(
            IDependencyResolver resolver,
            Action<IDependencyResolver, IEnumerable<Confluent.Kafka.TopicPartitionOffset>> handler)
        {
            if (!this.offsetsToCommit.IsEmpty)
            {
                handler(
                    resolver,
                    this.offsetsToCommit.Values.Select(
                        x =>
                            new Confluent.Kafka.TopicPartitionOffset(x.Topic, x.Partition, x.Offset)));
            }
        }

        private void CommitHandler()
        {
            lock (this.commitSyncRoot)
            {
                ConcurrentDictionary<(string, int), TopicPartitionOffset> offsets = null;

                try
                {
                    if (!this.offsetsToCommit.Any())
                    {
                        return;
                    }

                    offsets = Interlocked.Exchange(
                        ref this.offsetsToCommit,
                        new ConcurrentDictionary<(string, int), TopicPartitionOffset>());

                    this.consumer.Commit(
                        offsets.Values
                            .Select(x => new Confluent.Kafka.TopicPartitionOffset(x.Topic, x.Partition, x.Offset + 1))
                            .ToList());

                    if (!this.consumer.Configuration.ManagementDisabled)
                    {
                        this.LogOffsetsCommitted(offsets.Values);
                    }
                }
                catch (Exception e)
                {
                    this.logHandler.Warning(
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
            this.logHandler.Verbose(
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
                this.offsetsToCommit.TryAdd((tpo.Topic, tpo.Partition), tpo);
            }
        }
    }
}
