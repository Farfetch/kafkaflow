namespace KafkaFlow.Serializer.ProtoBuf
{
    using System;
    using System.IO;
    using global::ProtoBuf;

    /// <summary>
    /// Serializer class for messages using protobuf protocol
    /// </summary>
    public abstract class ProtobufMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// Creates a protocol-buffer representation of the given instance
        /// </summary>
        /// <param name="message">The message to be serialized (cannot be null)</param>
        /// <returns>The serialized message</returns>
        public byte[] Serialize(object message)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        /// <summary>
        /// Creates a new instance from a protocol-buffer representation
        /// </summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The type to be created</param>
        /// <returns>The deserialized message</returns>
        public object Deserialize(byte[] data, Type type)
        {
            using var stream = new MemoryStream(data);
            return Serializer.Deserialize(type, stream);
        }
    }
}
