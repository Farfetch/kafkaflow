namespace KafkaFlow
{
    using Confluent.SchemaRegistry;
    using Newtonsoft.Json;

    internal class ConfluentAvroTypeNameResolver : ISchemaRegistryTypeNameResolver
    {
        private readonly ISchemaRegistryClient client;

        public ConfluentAvroTypeNameResolver(ISchemaRegistryClient client)
        {
            this.client = client;
        }

        public string Resolve(int id)
        {
            var schema = this.client
                .GetSchemaAsync(id)
                .GetAwaiter()
                .GetResult();

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
