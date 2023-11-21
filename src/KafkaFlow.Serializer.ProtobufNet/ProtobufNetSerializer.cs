using System.IO;
using System.Threading.Tasks;

namespace KafkaFlow.Serializer
{
    /// <summary>
    /// A message serializer using protobuf-net library
    /// </summary>
    public class ProtobufNetSerializer : ISerializer
    {
        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            ProtoBuf.Serializer.Serialize(output, message);

            return Task.CompletedTask;
        }
    }
}
