namespace KafkaFlow.Serializer.Json
{
    using System;
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// A message serializer using System.Text.Json library
    /// </summary>
    public class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions options;

        /// <summary>
        /// </summary>
        /// <param name="options">Json serializer options</param>
        public JsonMessageSerializer(JsonSerializerOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc/>
        public JsonMessageSerializer() : this(new JsonSerializerOptions())
        {
        }

        /// <inheritdoc/>
        public void Serialize(object message, Stream output, SerializationContext context)
        {
            using var writer = new Utf8JsonWriter(output);
            JsonSerializer.Serialize(writer, message, this.options);
        }

        /// <inheritdoc/>
        public object Deserialize(Stream input, Type type, SerializationContext context)
        {
            return JsonSerializer.DeserializeAsync(input, type, this.options).GetAwaiter().GetResult();
        }
    }
}
