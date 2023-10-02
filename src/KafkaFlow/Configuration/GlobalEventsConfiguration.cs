namespace KafkaFlow.Configuration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class GlobalEventsConfiguration
{
    public GlobalEventsConfiguration(IReadOnlyCollection<Func<IMessageContext, IDependencyResolver, Task>> messageConsumeStartEvents)
    {
        this.MessageConsumeStartEvents = messageConsumeStartEvents;
    }

    public IReadOnlyCollection<Func<IMessageContext, IDependencyResolver, Task>> MessageConsumeStartEvents { get; }
}
