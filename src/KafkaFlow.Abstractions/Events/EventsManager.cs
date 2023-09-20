using System;
using KafkaFlow.Events.Args;

namespace KafkaFlow.Events
{
    public class EventsManager : IEventsListener, IEventsNotifier
    {
        public event EventHandler<ConsumeStartEventArgs> OnConsumeStart;

        public event EventHandler<ProduceStartEventArgs> OnProduceStart;

        public event EventHandler<ConsumeErrorEventArgs> OnConsumeError;

        public event EventHandler<ProduceErrorEventArgs> OnProduceError;

        public void NotifyOnConsumeError(Exception exception)
        {
            this.OnConsumeError?.Invoke(this, new ConsumeErrorEventArgs(exception));
        }

        public void NotifyOnConsumeStart(IMessageContext context)
        {
            this.OnConsumeStart?.Invoke(this, new ConsumeStartEventArgs(context));
        }

        public void NotifyOnProduceError(Exception exception)
        {
            this.OnProduceError?.Invoke(this, new ProduceErrorEventArgs(exception));
        }

        public void NotifyOnProduceStart(IMessageContext context)
        {
            this.OnProduceStart?.Invoke(this, new ProduceStartEventArgs(context));
        }
    }
}
