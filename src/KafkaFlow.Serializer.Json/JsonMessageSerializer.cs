namespace KafkaFlow.Serializer.Json
{
    using System;
    using System.Text;
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

        /// <summary>
        /// </summary>
        public JsonMessageSerializer() : this(new JsonSerializerOptions())
        {
        }

        /// <summary>Serializes the message</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>A UTF8 JSON string</returns>
        public byte[] Serialize(object message)
        {
            var serialized = JsonSerializer.Serialize(message, this.options);
            return Encoding.UTF8.GetBytes(serialized);
        }

        /// <summary>Deserialize the message</summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The destination type</param>
        /// <returns>An instance of the passed type</returns>
        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data); 

            return JsonSerializer.Deserialize(json, type, this.options);
        }
    }
}
