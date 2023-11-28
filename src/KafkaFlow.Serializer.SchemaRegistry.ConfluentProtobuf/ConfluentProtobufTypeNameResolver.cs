using System.Linq;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace KafkaFlow.Serializer.SchemaRegistry
{
    internal class ConfluentProtobufTypeNameResolver : IConfluentProtobufTypeNameResolver
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

            return BuildTypeName(protoFields);
        }

        private static string BuildTypeName(FileDescriptorProto protoFields)
        {
            var package = protoFields.Package;
            if (string.IsNullOrEmpty(package))
            {
                package = protoFields.Options.CsharpNamespace;
            }

            return $"{package}.{protoFields.MessageType.FirstOrDefault()?.Name}";
        }
    }
}
