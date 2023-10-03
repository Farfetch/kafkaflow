namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an Event to be subscribed.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="handler">The handler to be called when the event is fired.</param>
        /// <returns>Event subscription reference</returns>
        IEventSubscription Subscribe(Func<Task> handler);
    }

    /// <summary>
    /// Represents an Event to be subscribed.
    /// </summary>
    /// <typeparam name="TArg">The argument expected by the event.</typeparam>
    public interface IEvent<TArg>
    {
        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="handler">The handler to be called when the event is fired.</param>
        /// <returns>Event subscription reference</returns>
        IEventSubscription Subscribe(Func<TArg, Task> handler);
    }
}
