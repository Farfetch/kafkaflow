namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    public static class ProducerConfigurationBuilderExtensions
    {
        public static IProducerConfigurationBuilder WithThrottlingWhenLagExceedsThreshold(
            this IProducerConfigurationBuilder producer,
            string topicToObserve,
            string consumerGroupToObserve,
            long lagThreshold,
            TimeSpan? frequencyObserve = null,
            TimeSpan? delay = null)
        {
            frequencyObserve ??= TimeSpan.FromSeconds(5);
            delay ??= TimeSpan.FromSeconds(10);

            return producer.AddMiddlewares(builder =>
            {
                builder.Add(
                    resolver => new DelayMiddleware(
                        topicToObserve,
                        consumerGroupToObserve,
                        lagThreshold,
                        frequencyObserve.Value,
                        delay.Value,
                        resolver.Resolve<ILagReader>(),
                        resolver.Resolve<ILogHandler>()));
            });
        }

        public static IProducerConfigurationBuilder PausingConsumerWhenLagExceedsThreshold(
            this IProducerConfigurationBuilder producer,
            string topicToObserve,
            string consumerGroupToObserve,
            long lagThreshold,
            string consumerName,
            TimeSpan? frequencyObserve = null)
        {
            frequencyObserve ??= TimeSpan.FromSeconds(5);

            return producer.AddMiddlewares(builder =>
            {
                builder.Add(
                    resolver => new PauseConsumerMiddleware(
                        topicToObserve,
                        consumerGroupToObserve,
                        lagThreshold,
                        frequencyObserve.Value,
                        resolver.Resolve<IConsumerAccessor>().GetConsumer(consumerName),
                        resolver.Resolve<ILagReader>(),
                        resolver.Resolve<ILogHandler>()));
            });
        }
    }
}
