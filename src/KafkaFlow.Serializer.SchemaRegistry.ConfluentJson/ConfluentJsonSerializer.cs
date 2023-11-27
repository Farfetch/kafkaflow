﻿namespace KafkaFlow.Serializer.SchemaRegistry
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using NJsonSchema.Generation;

    /// <summary>
    /// A json message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentJsonSerializer : ISerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly JsonSerializerConfig serializerConfig;
        private readonly JsonSchemaGeneratorSettings schemaGeneratorSettings;

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
            this.schemaRegistryClient =
                resolver.Resolve<ISchemaRegistryClient>() ??
                throw new InvalidOperationException(
                    $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.serializerConfig = serializerConfig;
            this.schemaGeneratorSettings = schemaGeneratorSettings;
        }

        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            return ConfluentSerializerWrapper
                .GetOrCreateSerializer(
                    message.GetType(),
                    () => Activator.CreateInstance(
                        typeof(JsonSerializer<>).MakeGenericType(message.GetType()),
                        this.schemaRegistryClient,
                        this.serializerConfig,
                        this.schemaGeneratorSettings))
                .SerializeAsync(message, output, context);
        }
    }
}
