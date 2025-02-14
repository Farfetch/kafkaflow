using Confluent.Kafka;

namespace KafkaFlow.Consumers;

/// <summary>
/// Factory for creating an <see cref="ConsumerBuilder{TKey,TValue}"/>.
/// </summary>
public interface IConsumerBuilderFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ConsumerBuilder{TKey,TValue}"/>.
    /// </summary>
    /// <param name="config">The consumer config</param>
    /// <returns>An <see cref="ConsumerBuilder{TKey,TValue}"/>.</returns>
    IConsumerBuilder CreateConsumerBuilder(ConsumerConfig config);
}