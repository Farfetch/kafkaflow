using Confluent.SchemaRegistry;

namespace KafkaFlow;

/// <summary>
/// Factory for creating an <see cref="ISchemaRegistryClient"/>.
/// </summary>
public interface ISchemaRegistryClientFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISchemaRegistryClient"/>.
    /// </summary>
    /// <param name="config">The schema registry config</param>
    /// <returns>An <see cref="ISchemaRegistryClient"/>.</returns>
    ISchemaRegistryClient CreateSchemaRegistryClient(SchemaRegistryConfig config);
}