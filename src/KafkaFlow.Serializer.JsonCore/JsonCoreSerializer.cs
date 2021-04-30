namespace KafkaFlow.Serializer
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// A message serializer using System.Text.Json library
    /// </summary>
    public class JsonCoreSerializer : ISerializer
    {
        private readonly JsonSerializerOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        /// <param name="options">Json serializer options</param>
        public JsonCoreSerializer(JsonSerializerOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
        /// </summary>
        public JsonCoreSerializer()
            : this(new JsonSerializerOptions())
        {
        }

        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            using var writer = new Utf8JsonWriter(output);

            JsonSerializer.Serialize(writer, message, this.options);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return await JsonSerializer
                .DeserializeAsync(input, type, this.options)
                .ConfigureAwait(false);
        }
    }
}
