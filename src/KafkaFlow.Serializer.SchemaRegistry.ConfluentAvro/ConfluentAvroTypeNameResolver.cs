namespace KafkaFlow
{
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Newtonsoft.Json;

    internal class ConfluentAvroTypeNameResolver : IConfluentAvroTypeNameResolver
    {
        private readonly ISchemaRegistryClient client;

        public ConfluentAvroTypeNameResolver(ISchemaRegistryClient client)
        {
            this.client = client;
        }

        public async Task<string> ResolveAsync(int id)
        {
            var schema = await this.client.GetSchemaAsync(id);

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
