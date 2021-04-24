namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Confluent.Kafka;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer consumer;
        private readonly ILogHandler logHandler;

        private readonly Timer commitTimer;

        private ConcurrentDictionary<(string, int), TopicPartitionOffset> offsetsToCommit = new();
        private IProducer<byte[], byte[]> producer;
        private IConsumerProducerTransactionCoordinator consumerProducerTransactionCoordinator;
        private IConsumerGroupMetadata consumerGroupMetadata;

        public OffsetCommitter(
            IConsumer consumer,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.logHandler = logHandler;

            this.commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                consumer.Configuration.AutoCommitInterval,
                consumer.Configuration.AutoCommitInterval);
        }

        public void Dispose()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();
        }

        public void RegisterProducerConsumer(
            IProducer<byte[], byte[]> producer,
            IConsumerProducerTransactionCoordinator consumerProducerTransactionCoordinator,
            IConsumerContext consumerContext)
        {
            this.producer = producer;
            this.consumerProducerTransactionCoordinator = consumerProducerTransactionCoordinator;
            this.consumerGroupMetadata = consumerContext.ConsumerGroupMetadata;
        }

        public void StoreOffset(TopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition.Value),
                tpo,
                (_, _) => tpo);
        }

        private void CommitHandler()
        {
            if (this.offsetsToCommit.Count == 0)
            {
                return;
            }

            var offsets = this.offsetsToCommit.Values;
            this.offsetsToCommit = new ConcurrentDictionary<(string, int), TopicPartitionOffset>();

            try
            {
                if (this.producer != null)
                {
                    this.consumerProducerTransactionCoordinator.Initiated();
                    this.producer.SendOffsetsToTransaction(offsets, this.consumerGroupMetadata, Timeout.InfiniteTimeSpan);
                    this.producer.CommitTransaction();
                    this.producer.BeginTransaction();
                    this.consumerProducerTransactionCoordinator.Completed();
                }
                else
                {
                    this.consumer.Commit(offsets);
                }
            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error Committing offsets",
                    e,
                    null);
            }
        }
    }
}
