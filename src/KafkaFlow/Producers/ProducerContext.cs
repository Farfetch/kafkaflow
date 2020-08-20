namespace KafkaFlow.Producers
{
    using System.Threading;

    internal class ProducerContext : IProducerContext
    {
        public ProducerContext(string producerName, string topic, CancellationToken clientStopped)
        {
            this.ProducerName = producerName;
            this.Topic = topic;
            this.ClientStopped = clientStopped;
        }

        public string ProducerName { get; }

        public string Topic { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public CancellationToken ClientStopped { get; }
    }
}
