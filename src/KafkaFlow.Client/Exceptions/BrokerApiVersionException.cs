namespace KafkaFlow.Client.Exceptions
{
    using System;
    using KafkaFlow.Client.Protocol.Messages;

    /// <summary>
    /// Represents an error trying to get the broker api version
    /// </summary>
    public class BrokerApiVersionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerApiVersionException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code to associate with this exception</param>
        /// <param name="message">The message to associate with this exception</param>
        public BrokerApiVersionException(ErrorCode errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the error code for this exception
        /// </summary>
        public ErrorCode ErrorCode { get; }
    }
}
