namespace KafkaFlow.Serializer.ConfluentJson
{
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using NJsonSchema.Generation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A json message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentJsonSerializer : IMessageSerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly JsonSerializerConfig serializerConfig;
        private readonly JsonSchemaGeneratorSettings schemaGeneratorSettings;

        /// <summary>
        /// </summary>
        public ConfluentJsonSerializer(IDependencyResolver resolver) : this(resolver, new JsonSerializerConfig()) { }

        /// <summary>
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="serializerConfig"></param>
        public ConfluentJsonSerializer(IDependencyResolver resolver, JsonSerializerConfig serializerConfig)
            : this(resolver, serializerConfig, null) { }

        /// <summary>
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="serializerConfig"></param>
        /// <param name="schemaGeneratorSettings"></param>
        public ConfluentJsonSerializer(IDependencyResolver resolver,
            JsonSerializerConfig serializerConfig,
            JsonSchemaGeneratorSettings schemaGeneratorSettings)
        {
            schemaRegistryClient = resolver.Resolve<ISchemaRegistryClient>() ??
                                        throw new InvalidOperationException($"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.serializerConfig = serializerConfig;
            this.schemaGeneratorSettings = schemaGeneratorSettings;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            dynamic serializer = Activator.CreateInstance(
                    typeof(JsonSerializer<>).MakeGenericType(message.GetType()),
                    schemaRegistryClient,
                    serializerConfig,
                    schemaGeneratorSettings);

            return serializer
                .SerializeAsync(message as dynamic, SerializationContext.Empty)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public object Deserialize(byte[] message, Type type)
        {
            dynamic deserializer = Activator
                .CreateInstance(typeof(JsonDeserializer<>).MakeGenericType(type),
                        Enumerable.Empty<KeyValuePair<string, string>>(),
                        null);

            return deserializer
                .DeserializeAsync(message, message == null, SerializationContext.Empty)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}