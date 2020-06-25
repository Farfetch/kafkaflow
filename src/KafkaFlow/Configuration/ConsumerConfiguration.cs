namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal class ConsumerConfiguration
    {
        private readonly ConsumerConfig consumerConfig;

        public ConsumerConfiguration(
            ConsumerConfig consumerConfig,
            IEnumerable<string> topics,
            string consumerName,
            int workersCount,
            int bufferSize,
            Factory<IDistributionStrategy> distributionStrategyFactory,
            MiddlewareConfiguration middlewareConfiguration)
        {
            this.consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

            if (string.IsNullOrEmpty(this.consumerConfig.GroupId))
            {
                throw new ArgumentNullException(nameof(consumerConfig.GroupId));
            }

            this.DistributionStrategyFactory = distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
            this.MiddlewareConfiguration = middlewareConfiguration ?? throw new ArgumentNullException(nameof(middlewareConfiguration));
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.ConsumerName = consumerName ?? Guid.NewGuid().ToString();
            
            this.WorkersCount = workersCount > 0 ?
                workersCount :
                throw new ArgumentOutOfRangeException(
                    nameof(workersCount),
                    workersCount,
                    "The value must be greater than 0");
            
            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize),
                    bufferSize,
                    "The value must be greater than 0");
        }

        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; }

        public IEnumerable<string> Topics { get; }

        public string ConsumerName { get; }

        public int WorkersCount { get; }

        public string GroupId => this.consumerConfig.GroupId;

        public int BufferSize { get; }

        public bool AutoStoreOffsets { get; set; } = true;

        public ConsumerConfig GetKafkaConfig()
        {
            return this.consumerConfig;
        }
    }
}
