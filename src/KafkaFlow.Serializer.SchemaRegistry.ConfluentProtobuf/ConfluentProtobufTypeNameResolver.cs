extern alias GoogleProto;

using System.Linq;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;

namespace KafkaFlow;

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

        // Issue: https://github.com/confluentinc/confluent-kafka-dotnet/issues/2409
        var protoFields = GoogleProto::Google.Protobuf.Reflection.FileDescriptorProto.Parser.ParseFrom(GoogleProto::Google.Protobuf.ByteString.FromBase64(schemaString));

        return $"{protoFields.Package}.{protoFields.MessageType.FirstOrDefault()?.Name}";
    }
}
