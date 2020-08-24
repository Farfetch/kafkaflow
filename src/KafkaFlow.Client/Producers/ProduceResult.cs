namespace KafkaFlow.Client.Producers
{
    public readonly struct ProduceResult
    {
        public string Topic { get; }

        public int Partition { get; }

        public long Offset { get; }

        public ProduceData Data { get; }

        public ProduceResult(string topic, int partition, long offset, ProduceData data)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.Offset = offset;
            this.Data = data;
        }
    }
}
