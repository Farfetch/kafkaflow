using System;

namespace KafkaFlow
{
    /// <summary>
    /// Represents the errors in message context used in the events
    /// </summary>
    public class MessageErrorEventContext : MessageEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageErrorEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        /// <param name="exception">The event exception</param>
        public MessageErrorEventContext(IMessageContext messageContext, Exception exception)
            : base(messageContext)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the exception
        /// </summary>
        public Exception Exception { get; }
    }
}
