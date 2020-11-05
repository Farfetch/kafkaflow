namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using System.IO;
    using Avro.IO;
    using Avro.Specific;

    /// <summary>
    /// A message serializer using Apache.Avro library
    /// </summary>
    public class ApacheAvroMessageSerializer : IMessageSerializer
    {
        /// <summary>Serializes the message</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>The serialized message</returns>
        public byte[] Serialize(object message)
        {
            if (!(message is ISpecificRecord avroMessage))
            {
                throw new InvalidOperationException(
                    $"The message object {message.GetType().FullName} must implement the {nameof(ISpecificRecord)} interface");
            }

            using var ms = new MemoryStream();
            var writer = new SpecificDefaultWriter(avroMessage.Schema);
            writer.Write(avroMessage, new BinaryEncoder(ms));
            return ms.ToArray();
        }

        /// <summary>Deserialize the message </summary>
        /// <param name="data">The message to be deserialized</param>
        /// <param name="type">The destination type</param>
        /// <returns>The deserialized message</returns>
        public object Deserialize(byte[] data, Type type)
        {
            using var ms = new MemoryStream(data);

            if (!(Activator.CreateInstance(type) is ISpecificRecord message))
            {
                throw new InvalidOperationException(
                    $"The message object {type.FullName} must implement the {nameof(ISpecificRecord)} interface");
            }

            var reader = new SpecificDefaultReader(message.Schema, message.Schema);
            reader.Read(message, new BinaryDecoder(ms));
            return message;
        }
    }
}
