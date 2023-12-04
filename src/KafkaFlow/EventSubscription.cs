using System;

namespace KafkaFlow;

internal class EventSubscription : IEventSubscription
{
    private readonly Action _cancelDelegate;

    public EventSubscription(Action cancelDelegate)
    {
        _cancelDelegate = cancelDelegate;
    }

    public void Cancel()
    {
        _cancelDelegate.Invoke();
    }
}
