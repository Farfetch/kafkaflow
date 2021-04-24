namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    internal interface IOffsetManager
    {
        void StoreOffset(TopicPartitionOffset offset);

        void RegisterProducerConsumer(
            IProducer<byte[], byte[]> producer,
            IConsumerProducerTransactionCoordinator consumerProducerTransactionCoordinator,
            IConsumerContext consumerContext);
    }
}
