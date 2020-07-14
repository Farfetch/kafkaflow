namespace KafkaFlow.Producers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class ProducerManager : IProducerManager
    {
        private readonly ConcurrentDictionary<string, IMessageProducer> producers =
            new ConcurrentDictionary<string, IMessageProducer>();

        public IMessageProducer GetProducer(string name) =>
            this.producers.TryGetValue(name, out var consumer) ? consumer : null;

        public IMessageProducer GetProducer<TProducer>() =>
            this.producers.TryGetValue(typeof(TProducer).FullName, out var consumer) ? consumer : null;

        public IEnumerable<IMessageProducer> All => this.producers.Values;

        public IMessageProducer this[string name] => this.GetProducer(name);

        public void AddOrUpdate(IMessageProducer producer)
        {
            this.producers.AddOrUpdate(
                producer.ProducerName,
                producer,
                (x, y) => producer);
        }
    }
}
