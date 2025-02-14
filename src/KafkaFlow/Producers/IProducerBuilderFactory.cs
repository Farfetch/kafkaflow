using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// Factory for creating an <see cref="ProducerBuilder{TKey,TValue}"/>.
/// </summary>
public interface IProducerBuilderFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ProducerBuilder{TKey,TValue}"/>.
    /// </summary>
    /// <param name="config">The producer config</param>
    /// <returns>An <see cref="ProducerBuilder{TKey,TValue}"/>.</returns>
    IProducerBuilder CreateProducerBuilder(ProducerConfig config);
}