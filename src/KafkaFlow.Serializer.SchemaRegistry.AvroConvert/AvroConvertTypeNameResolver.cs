using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Newtonsoft.Json;

namespace KafkaFlow.Serializer.SchemaRegistry;

internal class AvroConvertTypeNameResolver : ISchemaRegistryTypeNameResolver, ISchemaStringTypeNameResolver
{
    private readonly ISchemaRegistryClient _client;

    public AvroConvertTypeNameResolver(ISchemaRegistryClient client)
    {
        _client = client;
    }

    public async Task<string> ResolveAsync(int id)
    {
        var schema = await _client.GetSchemaAsync(id);

        return GetSchemaTypeName(schema.SchemaString);
    }

    public string Resolve(string schemaString)
    {
        return GetSchemaTypeName(schemaString);
    }

    private string GetSchemaTypeName(string schemaString)
    {
        var avroFields = JsonConvert.DeserializeObject<AvroSchemaFields>(schemaString);

        return $"{avroFields.Namespace}.{avroFields.Name}";
    }

    private class AvroSchemaFields
    {
        public string Name { get; set; }

        public string Namespace { get; set; }
    }
}
