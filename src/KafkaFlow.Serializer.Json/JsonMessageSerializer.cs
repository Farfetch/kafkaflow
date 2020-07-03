namespace KafkaFlow.Serializer.Json
{
    using System;
    using System.Text;
    using System.Text.Json;

    /// <summary>
    /// Serializer class for messages using json protocol (using System.Text.Json library)
    /// </summary>
    public class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions options;

        /// <summary>
        /// JsonMessageSerializer constructor  
        /// </summary>
        /// <param name="options">Json serializer options</param>
        public JsonMessageSerializer(JsonSerializerOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// JsonMessageSerializer constructor
        /// </summary>
        public JsonMessageSerializer() : this(new JsonSerializerOptions())
        {
        }

        /// <summary>Creates a json representation of the given instance</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>A JSON string representation of the value</returns>
        public byte[] Serialize(object message)
        {
            var serialized = JsonSerializer.Serialize(message, this.options);
            return Encoding.UTF8.GetBytes(serialized);
        }

        /// <summary>Creates a new instance from a json representation</summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The type to be created</param>
        /// <returns>The deserialized message</returns>
        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonSerializer.Deserialize(json, type);
        }
    }
}
