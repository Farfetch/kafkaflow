namespace KafkaFlow.Consumers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class ConsumerManager : IConsumerManager, IConsumerAccessor
    {
        private readonly ConcurrentDictionary<string, IMessageConsumer> consumers =
            new ConcurrentDictionary<string, IMessageConsumer>();

        private ConsumerManager()
        {
        }

        public static readonly ConsumerManager Instance = new ConsumerManager();

        public IMessageConsumer GetConsumer(string name) =>
            this.consumers.TryGetValue(name, out var consumer) ? consumer : null;

        public IEnumerable<IMessageConsumer> All => this.consumers.Values;

        public void AddOrUpdateConsumer(IMessageConsumer consumer)
        {
            this.consumers.AddOrUpdate(
                consumer.ConsumerName,
                consumer,
                (x, y) => consumer);
        }
    }
}
