namespace KafkaFlow.SchemaRegistrySerializer
{
    using Confluent.SchemaRegistry;

    internal class RegisteredSchemaWrapper : IRegisteredSchema
    {
        private readonly RegisteredSchema schema;

        public RegisteredSchemaWrapper(RegisteredSchema schema)
        {
            this.schema = schema;
        }

        public int Id => this.schema.Id;

        public string Subject => this.schema.Subject;

        public int Version => this.schema.Version;

        public string SchemaString => this.schema.SchemaString;
    }
}
