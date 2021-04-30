namespace KafkaFlow.Serializer.SchemaRegistry
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;

    /// <summary>
    /// A message serializer using Apache.Avro library
    /// </summary>
    public class ConfluentAvroSerializer : ISerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly AvroSerializerConfig serializerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentAvroSerializer"/> class.
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        public ConfluentAvroSerializer(IDependencyResolver resolver)
            : this(resolver, new AvroSerializerConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentAvroSerializer"/> class.
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        /// <param name="serializerConfig">Avro serializer configuration</param>
        public ConfluentAvroSerializer(
            IDependencyResolver resolver,
            AvroSerializerConfig serializerConfig)
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
                        typeof(AvroSerializer<>).MakeGenericType(message.GetType()),
                        this.schemaRegistryClient,
                        this.serializerConfig))
                .SerializeAsync(message, output);
        }

        /// <inheritdoc/>
        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return ConfluentDeserializerWrapper
                .GetOrCreateDeserializer(
                    type,
                    () => Activator
                        .CreateInstance(
                            typeof(AvroDeserializer<>).MakeGenericType(type),
                            this.schemaRegistryClient,
                            new AvroDeserializerConfig()))
                .DeserializeAsync(input);
        }
    }
}
