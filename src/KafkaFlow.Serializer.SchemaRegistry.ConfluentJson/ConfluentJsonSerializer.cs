using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using NJsonSchema.Generation;

namespace KafkaFlow.Serializer.SchemaRegistry;

/// <summary>
/// A json message serializer integrated with the confluent schema registry
/// </summary>
public class ConfluentJsonSerializer : ISerializer
{
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly JsonSerializerConfig _serializerConfig;
    private readonly JsonSchemaGeneratorSettings _schemaGeneratorSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfluentJsonSerializer"/> class.
    /// </summary>
    /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
    /// <param name="serializerConfig">An instance of <see cref="JsonSerializerConfig"/></param>
    public ConfluentJsonSerializer(IDependencyResolver resolver, JsonSerializerConfig serializerConfig = null)
        : this(
            resolver,
            serializerConfig,
            null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfluentJsonSerializer"/> class.
    /// </summary>
    /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
    /// <param name="serializerConfig">An instance of <see cref="JsonSerializerConfig"/></param>
    /// <param name="schemaGeneratorSettings">An instance of <see cref="JsonSchemaGeneratorSettings"/></param>
    public ConfluentJsonSerializer(
        IDependencyResolver resolver,
        JsonSerializerConfig serializerConfig,
        JsonSchemaGeneratorSettings schemaGeneratorSettings = null)
    {
        _schemaRegistryClient =
            resolver.Resolve<ISchemaRegistryClient>() ??
            throw new InvalidOperationException(
                $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

        _serializerConfig = serializerConfig;
        _schemaGeneratorSettings = schemaGeneratorSettings;
    }

    /// <inheritdoc/>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        return ConfluentSerializerWrapper
            .GetOrCreateSerializer(
                message.GetType(),
                () => Activator.CreateInstance(
                    typeof(JsonSerializer<>).MakeGenericType(message.GetType()),
                    _schemaRegistryClient,
                    _serializerConfig,
                    _schemaGeneratorSettings))
            .SerializeAsync(message, output, context);
    }
}
