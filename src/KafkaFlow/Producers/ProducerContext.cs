namespace KafkaFlow.Producers
{
    internal class ProducerContext : IProducerContext
    {
        public ProducerContext(string topic)
        {
            this.Topic = topic;
        }

        public string Topic { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }
    }
}
