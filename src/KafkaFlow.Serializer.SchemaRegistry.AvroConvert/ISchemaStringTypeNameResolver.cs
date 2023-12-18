namespace KafkaFlow.Serializer.SchemaRegistry;

internal interface ISchemaStringTypeNameResolver
{
    string Resolve(string schemaString);
}
