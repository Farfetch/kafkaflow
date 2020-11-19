namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Buffers.Binary;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.IO;

    internal class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly ISchemaResolver schemaResolver;

        private static readonly RecyclableMemoryStreamManager MemoryStreamManager =
            new RecyclableMemoryStreamManager(
                1024 * 4,
                1024 * 1024 * 4,
                1024 * 1024 * 16,
                false);

        public SerializerProducerMiddleware(
            IMessageSerializer serializer,
            ISchemaResolver schemaResolver)
        {
            this.serializer = serializer;
            this.schemaResolver = schemaResolver;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (context.Message is null)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            var schema = await this.schemaResolver
                .ResolveAsync(context.Message.GetType())
                .ConfigureAwait(false);

            this.SerializeMessage(context, schema);

            context.Headers.SetString(
                "Schema-Reference",
                ContractNameParser.GetContractName(schema.Subject, schema.Version));

            await next(context).ConfigureAwait(false);
        }

        private void SerializeMessage(IMessageContext context, IRegisteredSchema schema)
        {
            using var stream = MemoryStreamManager.GetStream();

            WriteSchemaId(stream, schema.Id);

            var serializationContext = new SerializationContext(schema, null);
            this.serializer.Serialize(context.Message, stream, serializationContext);

            context.TransformMessage(stream.ToArray());
        }

        private static void WriteSchemaId(Stream stream, int schemaId)
        {
            var encodedId = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(encodedId, schemaId);

            stream.WriteByte(0);
            stream.Write(encodedId, 0, 4);
        }
    }
}
