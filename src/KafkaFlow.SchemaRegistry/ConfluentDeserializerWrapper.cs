using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.IO;

namespace KafkaFlow
{
    /// <summary>
    /// A wrapper to call the typed Confluent deserializers
    /// </summary>
    public abstract class ConfluentDeserializerWrapper
    {
        private static readonly RecyclableMemoryStreamManager s_memoryStreamManager = new();

        private static readonly ConcurrentDictionary<Type, ConfluentDeserializerWrapper> s_deserializers = new();

        /// <summary>
        /// Get the deserializer based on the target message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="deserializerFactory">A factory that creates a <see cref="IAsyncDeserializer{T}"/></param>
        /// <returns></returns>
        public static ConfluentDeserializerWrapper GetOrCreateDeserializer(
            Type messageType,
            Func<object> deserializerFactory)
        {
            return s_deserializers.SafeGetOrAdd(
                messageType,
                _ => (ConfluentDeserializerWrapper)Activator.CreateInstance(
                    typeof(InnerConfluentDeserializerWrapper<>).MakeGenericType(messageType),
                    deserializerFactory));
        }

        /// <summary>
        /// Deserialize a message using the passed deserializer
        /// </summary>
        /// <param name="input">The message stream to deserialize</param>
        /// <param name="context">Additional information provided for deserialization</param>
        /// <returns></returns>
        public abstract Task<object> DeserializeAsync(Stream input, ISerializerContext context);

        private class InnerConfluentDeserializerWrapper<T> : ConfluentDeserializerWrapper
        {
            private readonly IAsyncDeserializer<T> _deserializer;

            public InnerConfluentDeserializerWrapper(Func<object> deserializerFactory)
            {
                _deserializer = (IAsyncDeserializer<T>)deserializerFactory();
            }

            public override async Task<object> DeserializeAsync(Stream input, ISerializerContext context)
            {
                using var buffer = s_memoryStreamManager.GetStream();

                await input.CopyToAsync(buffer).ConfigureAwait(false);

                return await _deserializer
                    .DeserializeAsync(
                        new ReadOnlyMemory<byte>(buffer.GetBuffer(), 0, (int)buffer.Length),
                        false,
                        new SerializationContext(MessageComponentType.Value, context.Topic))
                    .ConfigureAwait(false);
            }
        }
    }
}
