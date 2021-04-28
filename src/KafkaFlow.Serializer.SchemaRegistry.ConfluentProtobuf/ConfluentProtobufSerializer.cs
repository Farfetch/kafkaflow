namespace KafkaFlow.Serializer.SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;

    /// <summary>
    /// A protobuf message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentProtobufSerializer : IMessageSerializer
    {
        private readonly ISchemaRegistryClient schemaRegistryClient;
        private readonly ProtobufSerializerConfig serializerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentProtobufSerializer"/> class.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
        public ConfluentProtobufSerializer(IDependencyResolver resolver)
            : this(resolver, new ProtobufSerializerConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentProtobufSerializer"/> class.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
        /// <param name="serializerConfig">An instance of <see cref="ProtobufSerializerConfig"/></param>
        public ConfluentProtobufSerializer(IDependencyResolver resolver, ProtobufSerializerConfig serializerConfig)
        {
            this.schemaRegistryClient =
                resolver.Resolve<ISchemaRegistryClient>() ??
                throw new InvalidOperationException(
                    $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

            this.serializerConfig = serializerConfig;
        }

        /// <inheritdoc/>
        public byte[] Serialize(object message)
        {
            return ConfluentSerializerWrapper
                .GetOrCreateSerializer(
                    message.GetType(),
                    () => Activator.CreateInstance(
                        typeof(ProtobufSerializer<>).MakeGenericType(message.GetType()),
                        this.schemaRegistryClient,
                        this.serializerConfig))
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
                            typeof(ProtobufDeserializer<>).MakeGenericType(type),
                            Enumerable.Empty<KeyValuePair<string, string>>()))
                .Deserialize(message);
        }
    }
}
