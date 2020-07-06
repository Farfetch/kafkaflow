namespace KafkaFlow.Serializer
{
    using System;

    /// <summary>
    /// Used by the serializer middleware to resolve the type when consuming and store it when producing
    /// </summary>
    public interface IMessageTypeResolver
    {
        /// <summary>
        /// Must return the message type when consuming
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        /// <returns></returns>
        Type OnConsume(IMessageContext context);
        
        /// <summary>
        /// Must store the message type somewhere
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        /// <returns></returns>
        void OnProduce(IMessageContext context);
    }
}
