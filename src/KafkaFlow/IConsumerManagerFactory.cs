namespace KafkaFlow
{
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    internal interface IConsumerManagerFactory
    {
        IConsumerManager Create(IConsumerConfiguration configuration, IDependencyResolver resolver);
    }
}
