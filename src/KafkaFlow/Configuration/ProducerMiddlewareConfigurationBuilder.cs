namespace KafkaFlow.Configuration
{
    internal class ProducerMiddlewareConfigurationBuilder
        : MiddlewareConfigurationBuilder<IProducerMiddlewareConfigurationBuilder, IProducerConfiguration>,
            IProducerMiddlewareConfigurationBuilder
    {
        public ProducerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
            : base(dependencyConfigurator)
        {
        }
    }
}
