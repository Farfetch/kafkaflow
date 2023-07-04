namespace KafkaFlow.Producers
{
    internal class ProducerContext : IProducerContext
    {
        public ProducerContext(string topic, IDependencyResolver producerDependencyResolver)
        {
            this.Topic = topic;
            this.ProducerDependencyResolver = producerDependencyResolver;
        }

        public string Topic { get; }

        public int? Partition { get; set; }

        public long? Offset { get; set; }

        public IDependencyResolver ProducerDependencyResolver { get; }
    }
}
