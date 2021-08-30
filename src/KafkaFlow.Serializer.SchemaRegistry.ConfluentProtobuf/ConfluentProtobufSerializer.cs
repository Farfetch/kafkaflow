namespace KafkaFlow.Serializer.SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;

    /// <summary>
    /// A protobuf message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentProtobufSerializer : ISerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly ProtobufSerializerConfig serializerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentProtobufSerializer"/> class.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
        /// <param name="serializerConfig">An instance of <see cref="ProtobufSerializerConfig"/></param>
        public ConfluentProtobufSerializer(IDependencyResolver resolver, ProtobufSerializerConfig serializerConfig = null)
        {
            this.schemaRegistryClient =
                resolver.Resolve<ISchemaRegistryClient>() ??
                throw new InvalidOperationException(
                    $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.serializerConfig = serializerConfig;
        }

        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            return ConfluentSerializerWrapper
                .GetOrCreateSerializer(
                    message.GetType(),
                    () => Activator.CreateInstance(
                        typeof(ProtobufSerializer<>).MakeGenericType(message.GetType()),
                        this.schemaRegistryClient,
                        this.serializerConfig))
                .SerializeAsync(message, output, context);
        }

        /// <inheritdoc/>
        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return ConfluentDeserializerWrapper
                .GetOrCreateDeserializer(
                    type,
                    () => Activator
                        .CreateInstance(
                            typeof(ProtobufDeserializer<>).MakeGenericType(type),
                            (IEnumerable<KeyValuePair<string, string>>)null))
                .DeserializeAsync(input, context);
        }
    }
}
