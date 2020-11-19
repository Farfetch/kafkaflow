namespace KafkaFlow
{
    /// <summary>
    /// Represents a schema definition
    /// </summary>
    public interface IRegisteredSchema
    {
        /// <summary>
        /// The schema Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The subject name registered in Kafka
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// The version number registered in Kafka
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The schema definition representation registered in Kafka
        /// </summary>
        string SchemaString { get; }
    }
}
