namespace KafkaFlow.Configuration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class GlobalEventsConfigurationBuilder : IGlobalEventsConfigurationBuilder
{
    private readonly List<Func<IMessageContext, IDependencyResolver, Task>> messageConsumeStartEvents = new();

    public GlobalEventsConfiguration Build()
    {
        return new GlobalEventsConfiguration(this.messageConsumeStartEvents);
    }

    public IGlobalEventsConfigurationBuilder SubscribeMessageConsumeStartEvent(Func<IMessageContext, IDependencyResolver, Task> handler)
    {
        this.messageConsumeStartEvents.Add(handler);
        return this;
    }
}
