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
    public class ConfluentAvroDeserializer : IDeserializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentAvroDeserializer"/> class.
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        public ConfluentAvroDeserializer(IDependencyResolver resolver)
        {
            this.schemaRegistryClient =
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
                            typeof(AvroDeserializer<>).MakeGenericType(type),
                            this.schemaRegistryClient,
                            null))
                .DeserializeAsync(input, context);
        }
    }
}
