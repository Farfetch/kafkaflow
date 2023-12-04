using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow;

/// <summary>
/// A wrapper to call the typed Confluent serializers and deserializers
/// </summary>
public abstract class ConfluentSerializerWrapper
{
    private static readonly ConcurrentDictionary<Type, ConfluentSerializerWrapper> s_serializers = new();

    /// <summary>
    /// Get the serializer based on the target message type
    /// </summary>
    /// <param name="messageType">The message type</param>
    /// <param name="serializerFactory">A factory that creates a <see cref="IAsyncSerializer{T}"/></param>
    /// <returns></returns>
    public static ConfluentSerializerWrapper GetOrCreateSerializer(
        Type messageType,
        Func<object> serializerFactory)
    {
        return s_serializers.SafeGetOrAdd(
            messageType,
            _ => (ConfluentSerializerWrapper)Activator.CreateInstance(
                typeof(InnerConfluentSerializerWrapper<>).MakeGenericType(messageType),
                serializerFactory));
    }

    /// <summary>
    /// Serialize a message using the passed serializer
    /// </summary>
    /// <param name="message">The message to serialize</param>
    /// <param name="output">Where the serialization result will be stored</param>
    /// <param name="context">Additional information provided for serialization</param>
    /// <returns></returns>
    public abstract Task SerializeAsync(object message, Stream output, ISerializerContext context);

    private class InnerConfluentSerializerWrapper<T> : ConfluentSerializerWrapper
    {
        private readonly IAsyncSerializer<T> _serializer;

        public InnerConfluentSerializerWrapper(Func<object> serializerFactory)
        {
            _serializer = (IAsyncSerializer<T>)serializerFactory();
        }

        public override async Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            var data = await _serializer
                .SerializeAsync((T)message, new SerializationContext(MessageComponentType.Value, context.Topic))
                .ConfigureAwait(false);

            await output
                .WriteAsync(data, 0, data.Length)
                .ConfigureAwait(false);
        }
    }
}
