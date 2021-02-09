namespace KafkaFlow.Serializer.ProtoBuf
{
    using System;
    using System.IO;
    using global::ProtoBuf;

    /// <summary>
    /// A message serializer using protobuf-net library
    /// </summary>
    public class ProtobufMessageSerializer : IMessageSerializer
    {
        /// <summary>Serializes the message</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>The serialized message</returns>
        public byte[] Serialize(object message)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        /// <summary>Deserialize the message </summary>
        /// <param name="data">The message to be deserialized</param>
        /// <param name="type">The destination type</param>
        /// <returns>The deserialized message</returns>
        public object Deserialize(byte[] data, Type type)
        {
            using var stream = new MemoryStream(data);
            return Serializer.Deserialize(type, stream);
        }
    }
}
