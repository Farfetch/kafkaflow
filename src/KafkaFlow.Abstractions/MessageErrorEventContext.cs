namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Represents the errors in message context used in the events
    /// </summary>
    public class MessageErrorEventContext : MessageEventContext
    {
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageErrorEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        /// <param name="exception">The event exception</param>
        public MessageErrorEventContext(IMessageContext messageContext, Exception exception)
            : base(messageContext)
        {
            this.exception = exception;
        }

        /// <summary>
        /// Gets the exception
        /// </summary>
        public Exception Exception => this.exception;
    }
}
