namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer consumer;
        private readonly IDependencyResolver resolver;

        private readonly
            IReadOnlyList<(Action<IDependencyResolver, IEnumerable<Confluent.Kafka.TopicPartitionOffset>> handler, TimeSpan interval)>
            pendingOffsetsHandlers;

        private readonly ILogHandler logHandler;

        private readonly object commitSyncRoot = new();

        private Timer commitTimer;
        private IReadOnlyList<Timer> statisticsTimers;

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> offsetsToCommit = new();

        public OffsetCommitter(
            IConsumer consumer,
            IDependencyResolver resolver,
            IReadOnlyList<(Action<IDependencyResolver, IEnumerable<Confluent.Kafka.TopicPartitionOffset>> handler, TimeSpan interval)>
                pendingOffsetsHandlers,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.resolver = resolver;
            this.pendingOffsetsHandlers = pendingOffsetsHandlers;
            this.logHandler = logHandler;
        }

        public void MarkAsProcessed(TopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition),
                tpo,
                (_, _) => tpo);
        }

        public Task StartAsync()
        {
            this.commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                this.consumer.Configuration.AutoCommitInterval,
                this.consumer.Configuration.AutoCommitInterval);

            this.statisticsTimers = this.pendingOffsetsHandlers
                .Select(
                    s => new Timer(
                        _ => this.PendingOffsetsHandler(this.resolver, s.handler),
                        null,
                        TimeSpan.Zero,
                        s.interval))
                .ToList();

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();

            foreach (var timer in this.statisticsTimers)
            {
                timer.Dispose();
            }

            return Task.CompletedTask;
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
