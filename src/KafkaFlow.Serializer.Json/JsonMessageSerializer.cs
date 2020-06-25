namespace KafkaFlow.Serializer.Json
{
    using System;
    using System.Text;
    using System.Text.Json;

    public class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions options;

        public JsonMessageSerializer(JsonSerializerOptions options)
        {
            this.options = options;
        }

        public JsonMessageSerializer()
        : this(new JsonSerializerOptions())
        {
        }

        public byte[] Serialize(object obj)
        {
            var serialized = JsonSerializer.Serialize(obj, this.options);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonSerializer.Deserialize(json, type);
        }
    }
}
