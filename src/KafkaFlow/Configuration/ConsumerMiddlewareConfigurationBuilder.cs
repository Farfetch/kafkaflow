namespace KafkaFlow.Configuration
{
    internal class ConsumerMiddlewareConfigurationBuilder
        : MiddlewareConfigurationBuilder<IConsumerMiddlewareConfigurationBuilder, IConsumerConfiguration>,
            IConsumerMiddlewareConfigurationBuilder
    {
        public ConsumerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
            : base(dependencyConfigurator)
        {
        }
    }
}
