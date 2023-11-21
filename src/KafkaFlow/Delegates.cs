using Confluent.Kafka;
using KafkaFlow.Consumers;

namespace KafkaFlow
{
    /// <summary>
    /// A factory to decorates the consumer created by KafkaFlow
    /// </summary>
    /// <param name="consumer">The consumer created by KafkaFlow</param>
    /// <param name="resolver">The <see cref="IDependencyResolver"/> to get registered services</param>
    public delegate IConsumer ConsumerCustomFactory(
        IConsumer consumer,
        IDependencyResolver resolver);

    /// <summary>
    /// A factory to decorates the producer created by KafkaFlow
    /// </summary>
    /// <param name="producer">The producer created by KafkaFlow</param>
    /// <param name="resolver">The <see cref="IDependencyResolver"/> to get registered services</param>
    public delegate IProducer<byte[], byte[]> ProducerCustomFactory(
        IProducer<byte[], byte[]> producer,
        IDependencyResolver resolver);
}
