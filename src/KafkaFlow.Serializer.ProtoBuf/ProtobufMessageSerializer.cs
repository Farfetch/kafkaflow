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
        /// <inheritdoc/>
        public void Serialize(object message, Stream output, global::KafkaFlow.SerializationContext context)
        {
            Serializer.Serialize(output, message);
        }
        
        /// <inheritdoc/>
        public object Deserialize(Stream input, Type type, global::KafkaFlow.SerializationContext context)
        {
            return Serializer.Deserialize(type, input);
        }
    }
}
