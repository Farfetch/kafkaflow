namespace KafkaFlow
{
    /// <summary>
    /// A struct with some metadata to be used in serialization
    /// </summary>
    public readonly struct SerializationContext
    {
        /// <summary>
        /// Schema used when producing messages with Schema Registry
        /// </summary>
        public IRegisteredSchema WriterSchema { get; }

        /// <summary>
        /// Schema used when consuming messages with Schema Registry
        /// </summary>
        public IRegisteredSchema ReaderSchema { get; }

        /// <summary>
        /// Creates a SerializationContext
        /// </summary>
        /// <param name="writerSchema">Schema used when producing messages with Schema Registry</param>
        /// <param name="readerSchema">Schema used when consuming messages with Schema Registry</param>
        public SerializationContext(IRegisteredSchema writerSchema, IRegisteredSchema readerSchema)
        {
            this.WriterSchema = writerSchema;
            this.ReaderSchema = readerSchema;
        }
    }
}
