using System.Collections.Generic;

namespace KafkaFlow.Consumers
{
    internal class ConsumerAccessor : IConsumerAccessor
    {
        private readonly IDictionary<string, IMessageConsumer> _consumers = new Dictionary<string, IMessageConsumer>();

        public IEnumerable<IMessageConsumer> All => _consumers.Values;

        public IMessageConsumer this[string name] => this.GetConsumer(name);

        public IMessageConsumer GetConsumer(string name) =>
            _consumers.TryGetValue(name, out var consumer) ? consumer : null;

        void IConsumerAccessor.Add(IMessageConsumer consumer) => _consumers.Add(consumer.ConsumerName, consumer);
    }
}
