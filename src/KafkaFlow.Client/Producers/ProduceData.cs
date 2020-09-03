namespace KafkaFlow.Client.Producers
{
    using KafkaFlow.Client.Protocol.Messages;

    public readonly struct ProduceData
    {
        public ProduceData(string topic, byte[] key, byte[] value) :
            this(topic, key, value, null)
        {
        }

        public ProduceData(string topic, byte[] key, byte[] value, Headers headers)
        {
            this.Topic = topic;
            this.Key = key;
            this.Value = value;
            this.Headers = headers;
        }

        public string Topic { get; }

        public byte[] Key { get; }

        public byte[] Value { get; }

        public Headers? Headers { get; }
    }
}
