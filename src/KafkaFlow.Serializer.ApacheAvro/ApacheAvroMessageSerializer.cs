namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using Avro.Specific;
    using Confluent.Kafka;
    using Confluent.Kafka.SyncOverAsync;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    
    /// <summary>
    /// A message serializer using Apache.Avro library
    /// </summary>
    public class ApacheAvroMessageSerializer : IMessageSerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly AvroSerializerConfig serializerConfig; 
        
        /// <summary>
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        public ApacheAvroMessageSerializer(IDependencyResolver resolver): 
            this(resolver, new AvroSerializerConfig())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        /// <param name="serializerConfig">Avro serializer configuration</param>
        public ApacheAvroMessageSerializer( 
            IDependencyResolver resolver,
            AvroSerializerConfig serializerConfig)
        {
            this.schemaRegistryClient = resolver.Resolve<ISchemaRegistryClient>() ?? 
                                        throw new InvalidOperationException($"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");
            
            this.serializerConfig = serializerConfig ;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            if (!(message is ISpecificRecord record))
            {
                throw new InvalidCastException($"The message type {message.GetType().FullName} must implement {nameof(ISpecificRecord)} interface.");
            }
            
            return new AvroSerializer<ISpecificRecord>(
                    this.schemaRegistryClient,
                    this.serializerConfig)
                .AsSyncOverAsync()
                .Serialize(record, SerializationContext.Empty);
        }

        /// <inheritdoc/>
        public object Deserialize(byte[] data, Type type)
        {
            dynamic deserializer = Activator
                    .CreateInstance(
                        typeof(AvroDeserializer<>).MakeGenericType(type),
                        this.schemaRegistryClient,
                        new AvroDeserializerConfig());
            
            return deserializer
                .DeserializeAsync(data, data == null, SerializationContext.Empty)
                .GetAwaiter()
                .GetResult();
        }
    }
}
