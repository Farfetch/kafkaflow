using System.Linq;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace KafkaFlow
{
    internal class ConfluentProtobufTypeNameResolver : ISchemaRegistryTypeNameResolver
    {
        private readonly ISchemaRegistryClient _client;

        public ConfluentProtobufTypeNameResolver(ISchemaRegistryClient client)
        {
            _client = client;
        }

        public async Task<string> ResolveAsync(int id)
        {
            var schemaString = (await _client.GetSchemaAsync(id, "serialized")).SchemaString;

            var protoFields = FileDescriptorProto.Parser.ParseFrom(ByteString.FromBase64(schemaString));

            return $"{protoFields.Package}.{protoFields.MessageType.FirstOrDefault()?.Name}";
        }
    }
}
