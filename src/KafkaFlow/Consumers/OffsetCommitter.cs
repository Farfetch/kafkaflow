namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Confluent.Kafka;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IKafkaConsumer consumer;
        private readonly ILogHandler logHandler;

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> offsetsToCommit =
            new ConcurrentDictionary<(string, int), TopicPartitionOffset>();

        private readonly Timer commitTimer;

        public OffsetCommitter(
            IKafkaConsumer consumer,
            TimeSpan autoCommitInterval,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.logHandler = logHandler;

            this.commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                autoCommitInterval,
                autoCommitInterval);
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
            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error Commiting Offsets",
                    e,
                    null);
            }
        }

        public void Dispose()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();
        }

        public void StoreOffset(TopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition.Value),
                tpo,
                (k, v) => tpo);
        }
    }
}
