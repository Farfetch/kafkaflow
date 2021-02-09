namespace KafkaFlow.Serializer.NewtonsoftJson
{
    using System;
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

        /// <summary>Serializes the message</summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>A UTF8 JSON string</returns>
        public byte[] Serialize(object message)
        {
            var serialized = JsonConvert.SerializeObject(message, this.settings);
            return Encoding.UTF8.GetBytes(serialized);
        }

        /// <summary>Deserialize the message</summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The destination type</param>
        /// <returns>An instance of the passed type</returns>
        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject(json, type, this.settings);
        }
    }
}
