using System;
using System.Collections.Generic;
using System.Text;
using KafkaFlow.Events.Args;

namespace KafkaFlow.Events
{
    public interface IEventsListener
    {
        event EventHandler<ConsumeStartEventArgs> OnConsumeStart;

        event EventHandler<ProduceStartEventArgs> OnProduceStart;

        event EventHandler<ConsumeErrorEventArgs> OnConsumeError;

        event EventHandler<ProduceErrorEventArgs> OnProduceError;
    }
}
