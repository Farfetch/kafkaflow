using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Consumers;

namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    internal class ConsumerThrottlingKafkaLagMetric : IConsumerThrottlingMetric
    {
        private readonly IReadOnlyList<IMessageConsumer> _consumers;

        public ConsumerThrottlingKafkaLagMetric(IConsumerAccessor consumerAccessor, IEnumerable<string> consumersNames)
        {
            _consumers = consumerAccessor.All
                .Where(consumer => consumersNames.Contains(consumer.ConsumerName))
                .ToList()
                .AsReadOnly();
        }

        public Task<long> GetValueAsync()
        {
            var lag = _consumers
                .SelectMany(x => x.GetTopicPartitionsLag())
                .Select(x => x.Lag)
                .Sum();

            return Task.FromResult(lag);
        }
    }
}
