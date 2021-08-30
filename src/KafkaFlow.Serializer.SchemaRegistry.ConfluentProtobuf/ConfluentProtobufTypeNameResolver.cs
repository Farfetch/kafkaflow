namespace KafkaFlow
{
    using System.Linq;
    using Confluent.SchemaRegistry;
    using Google.Protobuf;
    using Google.Protobuf.Reflection;

    internal class ConfluentProtobufTypeNameResolver : ISchemaRegistryTypeNameResolver
    {
        private readonly ISchemaRegistryClient client;

        public ConfluentProtobufTypeNameResolver(ISchemaRegistryClient client)
        {
            this.client = client;
        }

        public string Resolve(int id)
        {
            var schemaString = this.client
                .GetSchemaAsync(id, "serialized")
                .GetAwaiter()
                .GetResult()
                .SchemaString;

            var protoFields = FileDescriptorProto.Parser.ParseFrom(ByteString.FromBase64(schemaString));

            return $"{protoFields.Package}.{protoFields.MessageType.FirstOrDefault()?.Name}";
        }
    }
}
