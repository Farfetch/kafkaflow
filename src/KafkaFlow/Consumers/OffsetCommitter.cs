namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Confluent.Kafka;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer consumer;
        private readonly ILogHandler logHandler;

        private readonly Timer commitTimer;
        private readonly IReadOnlyList<Timer> statisticsTimers;

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> offsetsToCommit = new();

        public OffsetCommitter(
            IConsumer consumer,
            IDependencyResolver resolver,
            IReadOnlyList<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)>
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
                (tpo.Topic, tpo.Partition.Value),
                tpo,
                (_, _) => tpo);
        }

        private void PendingOffsetsHandler(
            IDependencyResolver resolver,
            Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler)
        {
            if (!this.offsetsToCommit.IsEmpty)
            {
                handler(resolver, this.offsetsToCommit.Values);
            }
        }

        private void CommitHandler()
        {
            if (!this.offsetsToCommit.Any())
            {
                return;
            }

            var offsets = this.offsetsToCommit;
            this.offsetsToCommit = new ConcurrentDictionary<(string, int), TopicPartitionOffset>();

            try
            {
                this.consumer.Commit(offsets.Values);

                if (!this.consumer.Configuration.ManagementDisabled)
                {
                    this.logHandler.Info(
                        "Committed offsets",
                        new
                        {
                            Offsets = offsets.GroupBy(
                                x => x.Key.Item1,
                                (topic, groupedOffsets) => new
                                {
                                    Topic = topic,
                                    Partitions = groupedOffsets.Select(
                                        offset => new
                                        {
                                            Partition = offset.Value.Partition.Value,
                                            Offset = offset.Value.Offset.Value,
                                        }),
                                }),
                        });
                }
            }
            catch (Exception e)
            {
                this.logHandler.Warning(
                    "Error Commiting Offsets",
                    new { ErrorMessage = e.Message });

                this.RequeueFailedOffsets(offsets.Values);
            }
        }

        private void RequeueFailedOffsets(IEnumerable<TopicPartitionOffset> offsets)
        {
            foreach (var tpo in offsets)
            {
                this.offsetsToCommit.TryAdd((tpo.Topic, tpo.Partition.Value), tpo);
            }
        }
    }
}
