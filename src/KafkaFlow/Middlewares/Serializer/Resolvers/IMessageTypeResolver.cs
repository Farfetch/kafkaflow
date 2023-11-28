using System;
using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.Serializer.Resolvers
{
    /// <summary>
    /// Used by the serializer middleware to resolve the type when consuming and store it when producing
    /// </summary>
    public interface IMessageTypeResolver
    {
        /// <summary>
        /// Returns the message type when consuming
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        /// <returns></returns>
        ValueTask<Type> OnConsumeAsync(IMessageContext context);

        /// <summary>
        /// Stores the message type somewhere when producing
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and the metadata</param>
        ValueTask OnProduceAsync(IMessageContext context);
    }
}
