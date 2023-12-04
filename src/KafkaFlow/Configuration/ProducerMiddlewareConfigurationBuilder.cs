namespace KafkaFlow.Configuration;

internal class ProducerMiddlewareConfigurationBuilder
    : MiddlewareConfigurationBuilder<IProducerMiddlewareConfigurationBuilder>,
        IProducerMiddlewareConfigurationBuilder
{
    public ProducerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        : base(dependencyConfigurator)
    {
    }
}
