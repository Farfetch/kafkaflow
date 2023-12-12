﻿using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFlow.Serializer.SchemaRegistry;

/// <summary>
/// A message serializer using Apache.Avro library
/// </summary>
public class ConfluentAvroSerializer : ISerializer
{
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly AvroSerializerConfig _serializerConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfluentAvroSerializer"/> class.
    /// </summary>
    /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
    /// <param name="serializerConfig">Avro serializer configuration</param>
    public ConfluentAvroSerializer(
        IDependencyResolver resolver,
        AvroSerializerConfig serializerConfig = null)
    {
        _schemaRegistryClient =
            resolver.Resolve<ISchemaRegistryClient>() ??
            throw new InvalidOperationException(
                $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

        _serializerConfig = serializerConfig;
    }

    /// <inheritdoc/>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        return ConfluentSerializerWrapper
            .GetOrCreateSerializer(
                message.GetType(),
                () => Activator.CreateInstance(
                    typeof(AvroSerializer<>).MakeGenericType(message.GetType()),
                    _schemaRegistryClient,
                    _serializerConfig))
            .SerializeAsync(message, output, context);
    }
}
