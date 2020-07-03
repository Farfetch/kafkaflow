namespace KafkaFlow.Serializer.NewtonsoftJson
{
    using System;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Serializer class for messages using json protocol (using Newtonsoft library)
    /// </summary>
    public class NewtonsoftJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerSettings settings;

        /// <summary>
        /// NewtonsoftJsonMessageSerializer constructor  
        /// </summary>
        /// <param name="settings">Json serializer settings</param>
        public NewtonsoftJsonMessageSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// NewtonsoftJsonMessageSerializer constructor  
        /// </summary>
        public NewtonsoftJsonMessageSerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>Creates a json representation of the given instance</summary>
        /// <param name="message">The message to be serialized (cannot be null)</param>
        /// <returns>The serialized message</returns>
        public byte[] Serialize(object message)
        {
            var serialized = JsonConvert.SerializeObject(message, this.settings);
            return Encoding.UTF8.GetBytes(serialized);
        }

        /// <summary>Creates a new instance from a json representation</summary>
        /// <param name="data">The message to be deserialized (cannot be null)</param>
        /// <param name="type">The type to be created.</param>
        /// <returns>The deserialized message</returns>
        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
