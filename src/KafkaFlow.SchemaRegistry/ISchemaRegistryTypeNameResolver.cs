namespace KafkaFlow
{
    /// <summary>
    /// Used to implement a type name resolver to messages using schema registry
    /// </summary>
    public interface ISchemaRegistryTypeNameResolver
    {
        /// <summary>
        /// Resolve the message type name of a schema
        /// </summary>
        /// <param name="schemaId">Identifier of the schema</param>
        /// <returns></returns>
        string Resolve(int schemaId);
    }
}
