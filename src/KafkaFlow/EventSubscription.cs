namespace KafkaFlow
{
    using System;

    internal class EventSubscription : IEventSubscription
    {
        private readonly Action cancelDelegate;

        public EventSubscription(Action cancelDelegate)
        {
            this.cancelDelegate = cancelDelegate;
        }

        public void Cancel()
        {
            this.cancelDelegate.Invoke();
        }
    }
}
