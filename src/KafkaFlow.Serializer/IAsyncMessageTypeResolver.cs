namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Used by the serializer middleware to resolve the type when consuming and store it when producing
    /// </summary>
    public interface IAsyncMessageTypeResolver
    {
        /// <summary>
        /// Returns the message type when consuming
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        /// <returns></returns>
        Task<Type> OnConsumeAsync(IMessageContext context);

        /// <summary>
        /// Stores the message type somewhere when producing
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        Task OnProduceAsync(IMessageContext context);
    }
}
