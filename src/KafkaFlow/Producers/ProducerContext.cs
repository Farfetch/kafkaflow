namespace KafkaFlow.Producers
{
    using System.Threading;

    internal class ProducerContext : IProducerContext
    {
        public ProducerContext(string topic, CancellationToken clientStopped)
        {
            this.Topic = topic;
            this.ClientStopped = clientStopped;
        }

        public string Topic { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public CancellationToken ClientStopped { get; }
    }
}
