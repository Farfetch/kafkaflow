namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Consumers;

    internal class ConsumerThrottlingKafkaLagMetric : IConsumerThrottlingMetric
    {
        private readonly IReadOnlyList<IMessageConsumer> consumers;

        public ConsumerThrottlingKafkaLagMetric(IConsumerAccessor consumerAccessor, IEnumerable<string> consumersNames)
        {
            this.consumers = consumerAccessor.All
                .Where(consumer => consumersNames.Contains(consumer.ConsumerName))
                .ToList()
                .AsReadOnly();
        }

        public Task<long> GetValueAsync()
        {
            var lag = this.consumers
                .SelectMany(x => x.GetTopicPartitionsLag())
                .Select(x => x.Lag)
                .Sum();

            return Task.FromResult(lag);
        }
    }
}
