namespace KafkaFlow.Serializer.NewtonsoftJson
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// A message serializer using NewtonsoftJson library
    /// </summary>
    public class NewtonsoftJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerSettings settings;

        /// <summary>
        /// </summary>
        /// <param name="settings">Json serializer settings</param>
        public NewtonsoftJsonMessageSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// </summary>
        public NewtonsoftJsonMessageSerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <inheritdoc/>
        public void Serialize(object message, Stream output, SerializationContext context)
        {
            using var sw = new StreamWriter(output, Encoding.UTF8);
            var serializer = JsonSerializer.CreateDefault(this.settings);

            serializer.Serialize(sw, message);
        }

        /// <inheritdoc/>
        public object Deserialize(Stream input, Type type, SerializationContext context)
        {
            using var sr = new StreamReader(input, Encoding.UTF8);
            var serializer = JsonSerializer.CreateDefault(this.settings);

            return serializer.Deserialize(sr, type);
        }
    }
}
