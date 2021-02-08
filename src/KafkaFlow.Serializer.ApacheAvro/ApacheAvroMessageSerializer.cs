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
        private const string NoSchemaRegistryUrlMessage =
            "There was not defined a schema registry to be used by ApachevroSerializer. Use 'WithSchemaRegistry' option at cluster level with a valid url.";
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly AvroSerializerConfig serializerConfig; 
        
        /// <summary>
        /// </summary>
        /// <param name="schemaRegistryClient">Schema registry client</param>
        public ApacheAvroMessageSerializer(ISchemaRegistryClient schemaRegistryClient): 
            this(schemaRegistryClient, new AvroSerializerConfig())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="schemaRegistryClient">Schema registry client</param>
        /// <param name="serializerConfig">Avro serializer configuration</param>
        public ApacheAvroMessageSerializer(
            ISchemaRegistryClient schemaRegistryClient, 
            AvroSerializerConfig serializerConfig)
        {
            this.schemaRegistryClient = schemaRegistryClient ?? throw new InvalidOperationException(NoSchemaRegistryUrlMessage);
            this.serializerConfig = serializerConfig ;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            return new AvroSerializer<ISpecificRecord>(
                    this.schemaRegistryClient,
                    this.serializerConfig)
                .AsSyncOverAsync()
                .Serialize((ISpecificRecord) message, SerializationContext.Empty);
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
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
