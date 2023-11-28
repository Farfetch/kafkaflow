using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Newtonsoft.Json;

namespace KafkaFlow
{
    internal sealed class ConfluentAvroTypeNameResolver : IConfluentAvroTypeNameResolver
    {
        private readonly ISchemaRegistryClient _client;

        public ConfluentAvroTypeNameResolver(ISchemaRegistryClient client)
        {
            _client = client;
        }

        public async Task<string> ResolveAsync(int id)
        {
            var schema = await _client.GetSchemaAsync(id);

            var avroFields = JsonConvert.DeserializeObject<AvroSchemaFields>(schema.SchemaString);
            return $"{avroFields.Namespace}.{avroFields.Name}";
        }

        private class AvroSchemaFields
        {
            public string Name { get; set; }

            public string Namespace { get; set; }
        }
    }
}
