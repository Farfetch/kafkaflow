using Confluent.SchemaRegistry;

namespace KafkaFlow;

internal class SchemaRegistryClientFactory : ISchemaRegistryClientFactory
{
    public ISchemaRegistryClient CreateSchemaRegistryClient(SchemaRegistryConfig config)
    {
        return new CachedSchemaRegistryClient(config);
    }
}