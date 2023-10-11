namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Provides access to events fired by the internals of the library
    /// </summary>
    public interface IGlobalEvents
    {
        /// <summary>
        /// Gets the message consume completed event
        /// </summary>
        IEvent<MessageEventContext> MessageConsumeCompleted { get; }

        /// <summary>
        /// Gets the message consume error event
        /// </summary>
        IEvent<MessageEventExceptionContext> MessageConsumeError { get; }

        /// <summary>
        /// Gets the message consume started event
        /// </summary>
        IEvent<MessageEventContext> MessageConsumeStarted { get; }

        /// <summary>
        /// Gets the message produce completed event
        /// </summary>
        IEvent<MessageEventContext> MessageProduceCompleted { get; }

        /// <summary>
        /// Gets the message produce error event
        /// </summary>
        IEvent<MessageEventExceptionContext> MessageProduceError { get; }

        /// <summary>
        /// Gets the message produce started event
        /// </summary>
        IEvent<MessageEventContext> MessageProduceStarted { get; }
    }
}
