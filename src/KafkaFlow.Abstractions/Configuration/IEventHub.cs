namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Provides access to events fired by the internals of the library
    /// </summary>
    public interface IEventHub
    {
        /// <summary>
        /// Gets the message consume started event
        /// </summary>
        IEvent<MessageEventContext> MessageConsumeStarted { get; }

        /// <summary>
        /// Gets the message consume ended event
        /// </summary>
        IEvent<MessageEventContext> MessageConsumeEnded { get; }

        /// <summary>
        /// Gets the message produce started event
        /// </summary>
        IEvent<MessageEventContext> MessageProduceStarted { get; }
    }
}
