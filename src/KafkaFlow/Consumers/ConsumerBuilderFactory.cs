using Confluent.Kafka;

namespace KafkaFlow.Consumers;

internal class ConsumerBuilderFactory : IConsumerBuilderFactory
{
    public IConsumerBuilder CreateConsumerBuilder(ConsumerConfig config)
    {
        return new ConsumerBuilderWrapper(config);
    }
}