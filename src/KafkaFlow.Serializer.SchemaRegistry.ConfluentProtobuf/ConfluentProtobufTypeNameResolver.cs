namespace KafkaFlow
{
    using System.Linq;
    using System.Text.RegularExpressions;
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

            return $"{ConvertToPascalCase(protoFields.Package)}.{protoFields.MessageType.FirstOrDefault()?.Name}";
        }

        private static string ConvertToPascalCase(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return identifier;
            }

            if (Regex.IsMatch(identifier, "^[_A-Z0-9]*$"))
            {
                return Regex.Replace(
                    identifier,
                    "(^|_)([A-Z0-9])([A-Z0-9]*)",
                    match => match.Groups[2].Value.ToUpperInvariant() + match.Groups[3].Value.ToLowerInvariant());
            }

            if (Regex.IsMatch(identifier, "^[_a-z0-9]*$"))
            {
                return Regex.Replace(
                    identifier,
                    "(^|_)([a-z0-9])([a-z0-9]*)",
                    match => match.Groups[2].Value.ToUpperInvariant() + match.Groups[3].Value.ToLowerInvariant());
            }

            return identifier.Replace("_", string.Empty);
        }
    }
}
