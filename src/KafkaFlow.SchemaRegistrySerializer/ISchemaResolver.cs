namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Threading.Tasks;

    internal interface ISchemaResolver
    {
        ValueTask<IRegisteredSchema> ResolveAsync(Type messageType);
    }
}
