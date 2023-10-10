namespace KafkaFlow
{
    /// <summary>
    /// Represents a message context used in the events
    /// </summary>
    public class MessageEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        public MessageEventContext(IMessageContext messageContext)
        {
            this.MessageContext = messageContext;
        }

        /// <summary>
        /// Gets the message context
        /// </summary>
        public IMessageContext MessageContext { get; }
    }
}
