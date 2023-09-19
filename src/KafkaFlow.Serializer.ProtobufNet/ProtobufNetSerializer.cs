namespace KafkaFlow.Serializer
{
    using System.IO;
    using System.Threading.Tasks;
    using ProtoBuf;

    /// <summary>
    /// A message serializer using protobuf-net library
    /// </summary>
    public class ProtobufNetSerializer : ISerializer
    {
        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            Serializer.Serialize(output, message);

            return Task.CompletedTask;
        }
    }
}
