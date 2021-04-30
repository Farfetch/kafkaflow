namespace KafkaFlow
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    /// <summary>
    /// A wrapper to call the typed Confluent serializers and deserializers
    /// </summary>
    public abstract class ConfluentSerializerWrapper
    {
        private static readonly ConcurrentDictionary<Type, ConfluentSerializerWrapper> Serializers = new();

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
            return Serializers.GetOrAdd(
                messageType,
                _ => (ConfluentSerializerWrapper) Activator.CreateInstance(
                    typeof(InnerConfluentSerializerWrapper<>).MakeGenericType(messageType),
                    serializerFactory));
        }

        /// <summary>
        /// Serialize a message using the passed serializer
        /// </summary>
        /// <param name="message">The message to serialize</param>
        /// <param name="output">Where the serialization result will be stored</param>
        /// <returns></returns>
        public abstract Task SerializeAsync(object message, Stream output);

        private class InnerConfluentSerializerWrapper<T> : ConfluentSerializerWrapper
        {
            private readonly IAsyncSerializer<T> serializer;

            public InnerConfluentSerializerWrapper(Func<object> serializerFactory)
            {
                this.serializer = (IAsyncSerializer<T>) serializerFactory();
            }

            public override async Task SerializeAsync(object message, Stream output)
            {
                var data = await this.serializer
                    .SerializeAsync((T) message, Confluent.Kafka.SerializationContext.Empty)
                    .ConfigureAwait(false);

                await output
                    .WriteAsync(data, 0, data.Length)
                    .ConfigureAwait(false);
            }
        }
    }
}
