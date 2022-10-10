namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;

    internal class ConsumerAdmin
        : IConsumerAdmin
    {
        private readonly IAdminProducer producer;

        public ConsumerAdmin(IAdminProducer producer)
        {
            this.producer = producer;
        }

        public async Task PauseConsumerGroupAsync(string groupId, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new PauseConsumersByGroup
                {
                    GroupId = groupId,
                    Topics = topics.ToList(),
                });
        }

        public async Task ResumeConsumerGroupAsync(string groupId, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new ResumeConsumersByGroup
                {
                    GroupId = groupId,
                    Topics = topics.ToList(),
                });
        }

        public async Task PauseConsumerAsync(string consumerName, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new PauseConsumerByName
                {
                    ConsumerName = consumerName,
                    Topics = topics.ToList(),
                });
        }

        public async Task ResumeConsumerAsync(string consumerName, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new ResumeConsumerByName
                {
                    ConsumerName = consumerName,
                    Topics = topics.ToList(),
                });
        }

        public async Task StartConsumerAsync(string consumerName)
        {
            await this.producer.ProduceAsync(new StartConsumerByName { ConsumerName = consumerName });
        }

        public async Task StopConsumerAsync(string consumerName)
        {
            await this.producer.ProduceAsync(new StopConsumerByName { ConsumerName = consumerName });
        }

        public async Task RestartConsumerAsync(string consumerName)
        {
            await this.producer.ProduceAsync(new RestartConsumerByName { ConsumerName = consumerName });
        }

        public async Task ResetOffsetsAsync(string consumerName, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new ResetConsumerOffset
                {
                    ConsumerName = consumerName,
                    Topics = topics.ToList(),
                });
        }

        public async Task RewindOffsetsAsync(string consumerName, DateTime pointInTime, IEnumerable<string> topics)
        {
            await this.producer.ProduceAsync(
                new RewindConsumerOffsetToDateTime
                {
                    ConsumerName = consumerName,
                    DateTime = pointInTime,
                    Topics = topics.ToList(),
                });
        }

        public async Task ChangeWorkersCountAsync(string consumerName, int workersCount)
        {
            await this.producer.ProduceAsync(
                new ChangeConsumerWorkersCount
                {
                    ConsumerName = consumerName,
                    WorkersCount = workersCount,
                });
        }
    }
}
