namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Buffers.Binary;
    using System.IO;
    using System.Threading.Tasks;

    internal class SerializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly ISchemaRegistryManager registryManager;
        private readonly ISchemaResolver schemaResolver;
        private readonly IMessageTypeFinder typeFinder;

        public SerializerConsumerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeFinder typeFinder,
            ISchemaRegistryManager registryManager,
            ISchemaResolver schemaResolver)
        {
            this.serializer = serializer;
            this.typeFinder = typeFinder;
            this.registryManager = registryManager;
            this.schemaResolver = schemaResolver;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (context.Message is null)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be deserialized and it is '{context.Message.GetType().FullName}'");
            }

            var schemaId = ReadSchemaId(rawData);

            var schema = await this.registryManager
                .GetSchemaAsync(schemaId)
                .ConfigureAwait(false);

            if (schema is null)
            {
                throw new InvalidOperationException($"There is no schema defined in Schema Registry with id {schemaId}");
            }

            var message = await this.DeserializeMessageAsync(rawData, schema).ConfigureAwait(false);

            context.TransformMessage(message);

            await next(context).ConfigureAwait(false);
        }

        private static int ReadSchemaId(byte[] data)
        {
            if (data[0] != 0)
            {
                throw new InvalidOperationException("Message not well formed to be used with Schema Registry");
            }

            return BinaryPrimitives.ReadInt32BigEndian(new ReadOnlySpan<byte>(data, 1, 4));
        }

        private async Task<object> DeserializeMessageAsync(byte[] rawData, IRegisteredSchema writerSchema)
        {
            //TODO: create a single type finder and multi-type finder
            var type = this.typeFinder.Find(writerSchema.Subject, writerSchema.Version);

            var readerSchema = await this.schemaResolver
                .ResolveAsync(type)
                .ConfigureAwait(false);

            using var stream = new MemoryStream(rawData)
            {
                Position = 5 // Skip Schema Registry information
            };

            var serializationContext = new SerializationContext(writerSchema, readerSchema);

            return this.serializer.Deserialize(stream, type, serializationContext);
        }
    }
}
