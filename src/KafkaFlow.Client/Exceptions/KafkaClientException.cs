namespace KafkaFlow.Client.Exceptions
{
    using System;

    /// <summary>
    /// An exception that represents any errors on Kafka client
    /// </summary>
    public class KafkaClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaClientException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public KafkaClientException(string message)
            : base(message)
        {
        }
    }
}
