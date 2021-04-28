namespace KafkaFlow.Serializer.SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using NJsonSchema.Generation;

    /// <summary>
    /// A json message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentJsonSerializer : IMessageSerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly JsonSerializerConfig serializerConfig;
        private readonly JsonSchemaGeneratorSettings schemaGeneratorSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentJsonSerializer"/> class.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
        public ConfluentJsonSerializer(IDependencyResolver resolver)
            : this(resolver, new JsonSerializerConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentJsonSerializer"/> class.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
        /// <param name="serializerConfig">An instance of <see cref="JsonSerializerConfig"/></param>
        public ConfluentJsonSerializer(IDependencyResolver resolver, JsonSerializerConfig serializerConfig)
            : this(resolver, serializerConfig, null)
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
            JsonSchemaGeneratorSettings schemaGeneratorSettings)
        {
            this.schemaRegistryClient =
                resolver.Resolve<ISchemaRegistryClient>() ??
                throw new InvalidOperationException(
                    $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.serializerConfig = serializerConfig;
            this.schemaGeneratorSettings = schemaGeneratorSettings;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            return ConfluentSerializerWrapper
                .GetOrCreateSerializer(
                    message.GetType(),
                    () => Activator.CreateInstance(
                        typeof(JsonSerializer<>).MakeGenericType(message.GetType()),
                        this.schemaRegistryClient,
                        this.serializerConfig,
                        this.schemaGeneratorSettings))
                .Serialize(message);
        }

        /// <inheritdoc/>
        public object Deserialize(byte[] message, Type type)
        {
            return ConfluentDeserializerWrapper
                .GetOrCreateDeserializer(
                    type,
                    () => Activator
                        .CreateInstance(
                            typeof(JsonDeserializer<>).MakeGenericType(type),
                            Enumerable.Empty<KeyValuePair<string, string>>(),
                            null))
                .Deserialize(message);
        }
    }
}
