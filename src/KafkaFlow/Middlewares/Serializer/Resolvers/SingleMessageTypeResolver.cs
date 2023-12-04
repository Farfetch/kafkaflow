using System;
using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.Serializer.Resolvers;

/// <summary>
/// The message type resolver to be used when all messages are the same type
/// </summary>
public class SingleMessageTypeResolver : IMessageTypeResolver
{
    private readonly Type _messageType;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleMessageTypeResolver"/> class.
    /// </summary>
    /// <param name="messageType">The message type to be returned when consuming</param>
    public SingleMessageTypeResolver(Type messageType)
    {
        _messageType = messageType;
    }

    /// <inheritdoc/>
    public ValueTask<Type> OnConsumeAsync(IMessageContext context) => new ValueTask<Type>(_messageType);

    /// <inheritdoc/>
    public ValueTask OnProduceAsync(IMessageContext context)
    {
        // Do nothing
        return default(ValueTask);
    }
}
