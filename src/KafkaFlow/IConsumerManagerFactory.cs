using KafkaFlow.Configuration;
using KafkaFlow.Consumers;

namespace KafkaFlow;

internal interface IConsumerManagerFactory
{
    IConsumerManager Create(IConsumerConfiguration configuration, IDependencyResolver resolver);
}
