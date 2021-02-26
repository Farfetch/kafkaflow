namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;

    internal class ConsumerAccessor : IConsumerAccessor
    {
        private readonly IDictionary<string, IMessageConsumer> consumers = new Dictionary<string, IMessageConsumer>();

        public IMessageConsumer GetConsumer(string name) =>
            this.consumers.TryGetValue(name, out var consumer) ? consumer : null;

        public IEnumerable<IMessageConsumer> All => this.consumers.Values;

        public IMessageConsumer this[string name] => this.GetConsumer(name);

        void IConsumerAccessor.Add(IMessageConsumer consumer) => this.consumers.Add(consumer.ConsumerName, consumer);
    }
}
