namespace KafkaFlow.Client.Producers
{
    public class ProduceData
    {
        public ProduceData(string topic, byte[] key, byte[] value)
        {
            this.Topic = topic;
            this.Key = key;
            this.Value = value;
        }

        public string Topic { get; }

        public byte[] Key { get; }

        public byte[] Value { get; }

        public IHeaders Headers { get; set; }
    }
}
