namespace KafkaFlow;

/// <summary>
/// Represents an Event subscription.
/// </summary>
public interface IEventSubscription
{
    /// <summary>
    /// Cancels the subscription to the event.
    /// </summary>
    void Cancel();
}
