namespace KafkaFlow
{
    /// <inheritdoc />
    public class SerializerContext : ISerializerContext
    {
        /// <summary>
        /// The static instance of the <see cref="SerializerContext"/>
        /// </summary>
        public static readonly ISerializerContext Empty = new SerializerContext();

        private SerializerContext()
        {
        }
    }
}
