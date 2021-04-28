namespace KafkaFlow
{
    using System;
    using System.Collections.Concurrent;
    using Confluent.Kafka;

    /// <summary>
    /// A wrapper to call the typed Confluent deserializers
    /// </summary>
    public abstract class ConfluentDeserializerWrapper
    {
        private static readonly ConcurrentDictionary<Type, ConfluentDeserializerWrapper> Deserializers = new();

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
            return Deserializers.GetOrAdd(
                messageType,
                _ => (ConfluentDeserializerWrapper) Activator.CreateInstance(
                    typeof(InnerConfluentDeserializerWrapper<>).MakeGenericType(messageType),
                    deserializerFactory));
        }

        /// <summary>
        /// Deserialize a message using the passed deserializer
        /// </summary>
        /// <param name="message">The message to deserialize</param>
        /// <returns></returns>
        public abstract object Deserialize(byte[] message);

        private class InnerConfluentDeserializerWrapper<T> : ConfluentDeserializerWrapper
        {
            private readonly IAsyncDeserializer<T> deserializer;

            public InnerConfluentDeserializerWrapper(Func<object> deserializerFactory)
            {
                this.deserializer = (IAsyncDeserializer<T>) deserializerFactory();
            }

            public override object Deserialize(byte[] message)
            {
                return this.deserializer
                    .DeserializeAsync(message, message == null, SerializationContext.Empty)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
