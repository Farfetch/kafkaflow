namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using Avro.Specific;
    using Configuration;
    using Confluent.Kafka;
    using Confluent.Kafka.SyncOverAsync;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    /// <summary>
    /// A message serializer using Apache.Avro library
    /// </summary>
    public class ApacheAvroMessageSerializer : IMessageSerializer
    {
        private const string NoSchemaRegistryUrlMessage =
            "There is no schema registry url defined in schema registry configuration. Use 'WithSchemaRegistry' option at cluster level with a non empty url.";

        private ISchemaRegistryClient schemaRegistryClient;

        private AvroSerializerConfig serializerConfig; 
        
        /// <summary>
        /// </summary>
        /// <param name="config">Avro serializer configuration</param>
        public ApacheAvroMessageSerializer(AvroSerializerConfig config)
        {
            this.serializerConfig = config;
        }

        /// <summary>
        /// </summary>
        public ApacheAvroMessageSerializer() : this(new AvroSerializerConfig())
        {
        }
        
        
        /// <inheritdoc/>
        public byte[] Serialize(object message, Configuration.SchemaRegistryConfiguration schemaRegistryConfiguration)
        {
            this.BuildSchemaRegistryClient(schemaRegistryConfiguration);

            return new AvroSerializer<ISpecificRecord>(
                    this.schemaRegistryClient,
                    this.serializerConfig)
                .AsSyncOverAsync()
                .Serialize((ISpecificRecord) message, SerializationContext.Empty);
        }

        /// <inheritdoc/>
        public object Deserialize(byte[] data, Type type, Configuration.SchemaRegistryConfiguration schemaRegistryConfiguration)
        {
            this.BuildSchemaRegistryClient(schemaRegistryConfiguration);
            
            dynamic deserializer = Activator
                    .CreateInstance(
                        typeof(AvroDeserializer<>).MakeGenericType(type),
                        this.schemaRegistryClient,
                        new AvroDeserializerConfig());
            
            return deserializer
                .DeserializeAsync(data, data == null, SerializationContext.Empty)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        
        private void BuildSchemaRegistryClient(SchemaRegistryConfiguration schemaRegistryConfiguration)
        {
            if (this.schemaRegistryClient == null)
            {
                if (string.IsNullOrEmpty(schemaRegistryConfiguration?.Url))
                {
                    throw new InvalidOperationException(NoSchemaRegistryUrlMessage);
                }

                this.schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfiguration.ToConfluent());
            }
        }
    }
}
