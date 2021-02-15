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
            return ((ProducerConfigurationBuilder) builder).WithProducerConfig(config);
        }

        /// <summary>
        /// Sets configurations in the consumer based on a <see cref="P:Confluent.Kafka.ConsumerConfig"/> instance 
        /// </summary>
        /// <param name="builder">A class that implements <see cref="IConsumerConfigurationBuilder"/></param>
        /// <param name="config"><see cref="P:Confluent.Kafka.ConsumerConfig"/> instance</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithConsumerConfig(this IConsumerConfigurationBuilder builder, ConsumerConfig config)
        {
            return ((ConsumerConfigurationBuilder) builder).WithConsumerConfig(config);
        }

        /// <summary>
        /// Register a custom consumer factory to be internally used by the framework
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="decoratorFactory">The factory method</param>
        /// <returns></returns>
        public static IConsumerConfigurationBuilder WithCustomFactory(
            this IConsumerConfigurationBuilder builder,
            ConsumerCustomFactory decoratorFactory)
        {
            return ((ConsumerConfigurationBuilder) builder).WithCustomFactory(decoratorFactory);
        }

        /// <summary>
        /// Register a custom producer factory to be internally used by the framework
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="decoratorFactory">The factory method</param>
        /// <returns></returns>
        public static IProducerConfigurationBuilder WithCustomFactory(
            this IProducerConfigurationBuilder builder,
            ProducerCustomFactory decoratorFactory)
        {
            return ((ProducerConfigurationBuilder) builder).WithCustomFactory(decoratorFactory);
        }
    }
}
