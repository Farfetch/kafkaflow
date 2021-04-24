namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    internal interface IOffsetCommitter : IDisposable
    {
        void StoreOffset(TopicPartitionOffset tpo);

        void RegisterProducerConsumer(
            IProducer<byte[], byte[]> producer,
            IConsumerProducerTransactionCoordinator consumerProducerTransactionCoordinator,
            IConsumerContext consumerContext);
    }
}
