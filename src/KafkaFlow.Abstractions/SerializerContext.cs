namespace KafkaFlow
{
    /// <inheritdoc />
    public class SerializerContext : ISerializerContext
    {
        /// <summary>
        /// The static instance of the <see cref="SerializerContext"/>
        /// </summary>
        public static readonly ISerializerContext Empty = new SerializerContext();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerContext"/> class.
        /// </summary>
        /// <param name="topic"><inheritdoc cref="Topic"/></param>
        public SerializerContext(string topic)
        {
            this.Topic = topic;
        }

        private SerializerContext()
        {
        }

        /// <summary>
        /// Gets the topic associated with the message
        /// </summary>
        public string Topic { get; }
    }
}
