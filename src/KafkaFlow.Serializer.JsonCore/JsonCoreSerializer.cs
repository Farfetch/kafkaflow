namespace KafkaFlow.Serializer
{
    using System;
    using System.Text.Json;

    /// <summary>
    /// A message serializer using System.Text.Json library
    /// </summary>
    public class JsonCoreSerializer : IMessageSerializer
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

        /// <summary>Serializes the message</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>A UTF8 JSON string</returns>
        public byte[] Serialize(object message)
        {
            return JsonSerializer.SerializeToUtf8Bytes(message, this.options);
        }

        /// <summary>Deserialize the message</summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The destination type</param>
        /// <returns>An instance of the passed type</returns>
        public object Deserialize(byte[] data, Type type)
        {
            return JsonSerializer.Deserialize(data, type, this.options);
        }
    }
}
