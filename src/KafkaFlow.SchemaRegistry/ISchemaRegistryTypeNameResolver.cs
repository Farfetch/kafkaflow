namespace KafkaFlow
{
    using System.Threading.Tasks;

    /// <summary>
    /// An interface to implement a type name resolver to messages serialized with schema registry serializers
    /// </summary>
    public interface ISchemaRegistryTypeNameResolver
    {
        /// <summary>
        /// Resolve the message type name of a schema
        /// </summary>
        /// <param name="schemaId">Identifier of the schema</param>
        /// <returns></returns>
        Task<string> ResolveAsync(int schemaId);
    }
}
