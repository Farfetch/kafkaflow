namespace KafkaFlow.Serializer
{
    using System;

    /// <summary>
    /// The message type resolver to be used when all messages are the same type
    /// </summary>
    public class SingleMessageTypeResolver : IMessageTypeResolver
    {
        private readonly Type messageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleMessageTypeResolver"/> class.
        /// </summary>
        /// <param name="messageType">The message type to be returned when consuming</param>
        public SingleMessageTypeResolver(Type messageType)
        {
            this.messageType = messageType;
        }

        /// <inheritdoc/>
        public Type OnConsume(IMessageContext context) => this.messageType;

        /// <inheritdoc/>
        public void OnProduce(IMessageContext context)
        {
            // Do nothing
        }
    }
}
