namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A context that contains the message and metadata
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// Gets the message
        /// </summary>
        Message Message { get; }

        /// <summary>
        /// Gets the message headers
        /// </summary>
        IMessageHeaders Headers { get; }

        /// <summary>
        /// Gets the <see cref="IConsumerContext"></see> from the consumed message
        /// </summary>
        IConsumerContext ConsumerContext { get; }

        /// <summary>
        /// Gets the <see cref="IProducerContext"></see> from the produced message
        /// </summary>
        IProducerContext ProducerContext { get; }

        /// <summary>
        /// Creates a new <see cref="IMessageContext"/> with the new message
        /// </summary>
        /// <param name="key">The new message key</param>
        /// <param name="value">The new message value</param>
        /// <returns>A new message context containing the new values</returns>
        IMessageContext SetMessage(object key, object value);

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="message">key</param>
        /// <returns></returns>
        [Obsolete("This method should no longer be used, use the " + nameof(SetMessage) + "() instead.", true)]
        IMessageContext TransformMessage(object message);

        IDictionary<string, object> Items { get; }
    }
}
