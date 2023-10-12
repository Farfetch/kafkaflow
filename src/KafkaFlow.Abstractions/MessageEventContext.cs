namespace KafkaFlow
{
    /// <summary>
    /// Represents a message context used in the events
    /// </summary>
    public class MessageEventContext
    {
        private readonly IMessageContext messageContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        public MessageEventContext(IMessageContext messageContext)
        {
            this.messageContext = messageContext;
        }

        /// <summary>
        /// Gets the message context
        /// </summary>
        public IMessageContext MessageContext => this.messageContext;
    }
}
