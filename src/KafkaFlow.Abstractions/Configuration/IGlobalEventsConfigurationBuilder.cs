namespace KafkaFlow.Configuration;

using System;
using System.Threading.Tasks;

public interface IGlobalEventsConfigurationBuilder
{
    public IGlobalEventsConfigurationBuilder SubscribeMessageConsumeStartEvent(Func<IMessageContext, IDependencyResolver, Task> handler);
}
