namespace KafkaFlow.Serializer.NewtonsoftJson
{
    using System;
    using System.Text;
    using Newtonsoft.Json;

    public class NewtonsoftJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerSettings settings;

        public NewtonsoftJsonMessageSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        public NewtonsoftJsonMessageSerializer()
            : this(new JsonSerializerSettings())
        {
        }

        public byte[] Serialize(object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj, this.settings);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
