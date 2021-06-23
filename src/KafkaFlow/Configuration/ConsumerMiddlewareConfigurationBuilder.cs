namespace KafkaFlow.Configuration
{
    internal class ConsumerMiddlewareConfigurationBuilder
        : MiddlewareConfigurationBuilder<IConsumerMiddlewareConfigurationBuilder>,
            IConsumerMiddlewareConfigurationBuilder
    {
        public ConsumerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
            : base(dependencyConfigurator)
        {
        }
    }
}
