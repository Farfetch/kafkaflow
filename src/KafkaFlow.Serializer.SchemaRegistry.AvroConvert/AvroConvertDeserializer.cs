using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFlow.Serializer.SchemaRegistry;

/// <summary>
/// A message deserializer using AvroConvert library
/// </summary>
public class AvroConvertDeserializer : IDeserializer
{
    private readonly ISchemaRegistryClient _schemaRegistryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvroConvertDeserializer"/> class.
    /// </summary>
    /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
    public AvroConvertDeserializer(IDependencyResolver resolver)
    {
        _schemaRegistryClient =
            resolver.Resolve<ISchemaRegistryClient>() ??
            throw new InvalidOperationException(
                $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");
    }

    /// <inheritdoc/>
    public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
    {
        return ConfluentDeserializerWrapper
            .GetOrCreateDeserializer(
                type,
                () => Activator
                    .CreateInstance(
                        typeof(ReflectionDeserializer<>).MakeGenericType(type),
                        _schemaRegistryClient))
            .DeserializeAsync(input, context);
    }
}
