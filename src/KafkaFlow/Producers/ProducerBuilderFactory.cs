using Confluent.Kafka;

namespace KafkaFlow.Producers;

internal class ProducerBuilderFactory : IProducerBuilderFactory
{
    public IProducerBuilder CreateProducerBuilder(ProducerConfig config)
    {
        return new ProducerBuilderWrapper(config);
    }
}