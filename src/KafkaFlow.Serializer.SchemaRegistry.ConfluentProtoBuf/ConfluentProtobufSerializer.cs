namespace KafkaFlow.Serializer.ConfluentProtoBuf
{
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A protobuf message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentProtobufSerializer : IMessageSerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly ProtobufSerializerConfig schemaRegistrySerializerConfig;

        /// <summary>
        /// </summary>
        public ConfluentProtobufSerializer(IDependencyResolver resolver) : this(resolver, new ProtobufSerializerConfig()) { }

        /// <summary>
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="schemaRegistrySerializerConfig"></param>
        public ConfluentProtobufSerializer(IDependencyResolver resolver, ProtobufSerializerConfig schemaRegistrySerializerConfig)
        {
            schemaRegistryClient = resolver.Resolve<ISchemaRegistryClient>() ??
                                        throw new InvalidOperationException($"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.schemaRegistrySerializerConfig = schemaRegistrySerializerConfig;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            dynamic serializer = Activator.CreateInstance(
                    typeof(ProtobufSerializer<>).MakeGenericType(message.GetType()),
                    schemaRegistryClient,
                    schemaRegistrySerializerConfig);

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
                .CreateInstance(typeof(ProtobufDeserializer<>).MakeGenericType(type),
                    Enumerable.Empty<KeyValuePair<string, string>>());

            return deserializer
                .DeserializeAsync(message, message == null, SerializationContext.Empty)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}