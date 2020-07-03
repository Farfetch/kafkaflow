namespace KafkaFlow.Serializer
{
    using System;

    /// <summary>
    /// Represents the interface to be implemented by custom message type resolver implementations
    /// </summary>
    public interface IMessageTypeResolver
    {
        /// <summary>
        /// Gets the type of the message being consumed
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/> containing context data of message being consumed</param>
        /// <returns></returns>
        Type OnConsume(IMessageContext context);
        
        /// <summary>
        /// Sets the type of the message being produced
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/> containing context data of message being produced</param>
        /// <returns></returns>
        void OnProduce(IMessageContext context);
    }
}
