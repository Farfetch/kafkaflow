namespace KafkaFlow.Serializer
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ProtoBuf;

    /// <summary>
    /// A message deserializer using protobuf-net library
    /// </summary>
    public class ProtobufNetDeserializer : IDeserializer
    {
        /// <inheritdoc/>
        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return Task.FromResult(Serializer.Deserialize(type, input));
        }
    }
}
