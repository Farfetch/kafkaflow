namespace KafkaFlow
{
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    /// <summary>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        public static IProducerConfigurationBuilder WithProducerConfig(this IProducerConfigurationBuilder builder, ProducerConfig config)
        {
            ((ProducerConfigurationBuilder) builder).WithProducerConfig(config);
            return builder;
        }

        public static IConsumerConfigurationBuilder WithConsumerConfig(this IConsumerConfigurationBuilder builder, ConsumerConfig config)
        {
            ((ConsumerConfigurationBuilder)builder).WithConsumerConfig(config);
            return builder;
        }
    }
}
