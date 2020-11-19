namespace KafkaFlow.SchemaRegistrySerializer
{
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;

    internal interface ISchemaRegistryManager
    {
        Task<IRegisteredSchema> GetSchemaAsync(string subject, int version);

        ValueTask<IRegisteredSchema> GetSchemaAsync(int id);
    }
}
