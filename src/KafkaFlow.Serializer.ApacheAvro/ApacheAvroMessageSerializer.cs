namespace KafkaFlow.Serializer
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using Avro;
    using Avro.Generic;
    using Avro.IO;
    using Avro.Reflect;
    using Avro.Specific;

    /// <summary>
    /// A message serializer using Apache.Avro library
    /// </summary>
    public class ApacheAvroMessageSerializer : IMessageSerializer
    {
        private static readonly ConcurrentDictionary<int, Schema> ParsedSchemaCache = new ConcurrentDictionary<int, Schema>();

        private static readonly ClassCache ClassCache = new ClassCache();

        /// <inheritdoc/>
        public void Serialize(object message, Stream output, SerializationContext context)
        {
            var schema = TryGetMessageSchema(message, context.WriterSchema);
            var writer = CreateWriter(message, schema);
            writer.Write(message, new BinaryEncoder(output));
        }

        /// <inheritdoc/>
        public object Deserialize(Stream input, Type type, SerializationContext context)
        {
            var message = Activator.CreateInstance(type);
            var readerSchema = TryGetMessageSchema(message, context.ReaderSchema);
            var reader = CreateReader(message, GetParsedSchema(context.WriterSchema), readerSchema);

            return reader.Read(message, new BinaryDecoder(input));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DefaultReader CreateReader(object message, Schema writerSchema, Schema readerSchema)
        {
            return message switch
            {
                ISpecificRecord _ => new SpecificDefaultReader(writerSchema, readerSchema),
                GenericRecord _ => new DefaultReader(writerSchema, readerSchema),
                _ => new ReflectDefaultReader(message.GetType(), writerSchema, readerSchema, ClassCache)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DefaultWriter CreateWriter(object message, Schema schema)
        {
            return message switch
            {
                ISpecificRecord _ => new SpecificDefaultWriter(schema),
                GenericRecord _ => new DefaultWriter(schema),
                _ => new ReflectDefaultWriter(message.GetType(), schema, ClassCache)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Schema TryGetMessageSchema(object message, IRegisteredSchema registeredSchema)
        {
            Schema schema;

            if (registeredSchema != null)
            {
                schema = GetParsedSchema(registeredSchema);
            }
            else
            {
                schema = message switch
                {
                    ISpecificRecord record => record.Schema,
                    GenericRecord record => record.Schema,
                    _ => null
                };
            }

            if (schema is null)
            {
                throw new SerializationException(
                    $"To use ApacheAvro serializer the Producer/Consumer must use Schema Registry serializer or the message object must implement {nameof(ISpecificRecord)} or inherit from class {nameof(GenericRecord)}");
            }

            return schema;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Schema GetParsedSchema(IRegisteredSchema schema)
        {
            return ParsedSchemaCache.GetOrAdd(schema.Id, _ => Schema.Parse(schema.SchemaString));
        }
    }
}
