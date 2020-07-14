namespace KafkaFlow.Consumers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class ConsumerManager : IConsumerManager
    {
        private readonly ConcurrentDictionary<string, IMessageConsumer> consumers =
            new ConcurrentDictionary<string, IMessageConsumer>();

        public IMessageConsumer GetConsumer(string name) =>
            this.consumers.TryGetValue(name, out var consumer) ? consumer : null;

        public IEnumerable<IMessageConsumer> All => this.consumers.Values;

        public IMessageConsumer this[string name] => this.GetConsumer(name);

        public void AddOrUpdate(IMessageConsumer consumer)
        {
            this.consumers.AddOrUpdate(
                consumer.ConsumerName,
                consumer,
                (x, y) => consumer);
        }
    }
}
