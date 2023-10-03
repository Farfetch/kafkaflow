using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Configuration
{
    public interface IEventHub
    {
        IEvent<IMessageContext> MessageConsumeStarted { get; }
    }
}
