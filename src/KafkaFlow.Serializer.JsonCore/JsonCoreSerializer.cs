namespace KafkaFlow.Serializer
{
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// A message serializer using System.Text.Json library
    /// </summary>
    public class JsonCoreSerializer : ISerializer
    {
        private readonly JsonSerializerOptions serializerOptions;
        private readonly JsonWriterOptions writerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        /// <param name="options">Json serializer options</param>
        public JsonCoreSerializer(JsonSerializerOptions options)
        {
            this.serializerOptions = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        /// <param name="writerOptions">Json writer options</param>
        public JsonCoreSerializer(JsonWriterOptions writerOptions)
        {
            this.writerOptions = writerOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        /// <param name="serializerOptions">Json serializer options</param>
        /// <param name="writerOptions">Json writer options</param>
        public JsonCoreSerializer(JsonSerializerOptions serializerOptions, JsonWriterOptions writerOptions)
        {
            this.serializerOptions = serializerOptions;
            this.writerOptions = writerOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        public JsonCoreSerializer()
            : this(new JsonSerializerOptions(), default)
        {
        }

        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            using var writer = new Utf8JsonWriter(output, this.writerOptions);

            JsonSerializer.Serialize(writer, message, this.serializerOptions);

            return Task.CompletedTask;
        }
    }
}
