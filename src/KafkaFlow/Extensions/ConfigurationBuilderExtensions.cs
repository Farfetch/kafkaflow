namespace KafkaFlow
{
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Provides extension methods over <see cref="IConsumerConfigurationBuilder"/> and <see cref="IProducerConfigurationBuilder"/> 
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Sets configurations in the producer based on a <see cref="P:Confluent.Kafka.ProducerConfig"/> instance 
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IProducerConfigurationBuilder"/></param>
        /// <param name="config"><see cref="P:Confluent.Kafka.ProducerConfig"/> instance</param>
        /// <returns></returns>
        public static IProducerConfigurationBuilder WithProducerConfig(this IProducerConfigurationBuilder builder, ProducerConfig config)
        {
            ((ProducerConfigurationBuilder) builder).WithProducerConfig(config);
            return builder;
        }

        /// <summary>
        /// Sets configurations in the consumer based on a <see cref="P:Confluent.Kafka.ConsumerConfig"/> instance 
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IConsumerConfigurationBuilder"/></param>
        /// <param name="config"><see cref="P:Confluent.Kafka.ConsumerConfig"/> instance</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithConsumerConfig(this IConsumerConfigurationBuilder builder, ConsumerConfig config)
        {
            ((ConsumerConfigurationBuilder)builder).WithConsumerConfig(config);
            return builder;
        }
    }
}
