using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;

namespace KafkaFlow.Admin;

internal class ConsumerAdmin
    : IConsumerAdmin
{
    private readonly IAdminProducer _producer;

    public ConsumerAdmin(IAdminProducer producer)
    {
        _producer = producer;
    }

    public async Task PauseConsumerGroupAsync(string groupId, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new PauseConsumersByGroup
            {
                GroupId = groupId,
                Topics = topics.ToList(),
            });
    }

    public async Task ResumeConsumerGroupAsync(string groupId, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new ResumeConsumersByGroup
            {
                GroupId = groupId,
                Topics = topics.ToList(),
            });
    }

    public async Task PauseConsumerAsync(string consumerName, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new PauseConsumerByName
            {
                ConsumerName = consumerName,
                Topics = topics.ToList(),
            });
    }

    public async Task ResumeConsumerAsync(string consumerName, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new ResumeConsumerByName
            {
                ConsumerName = consumerName,
                Topics = topics.ToList(),
            });
    }

    public async Task StartConsumerAsync(string consumerName)
    {
        await _producer.ProduceAsync(new StartConsumerByName { ConsumerName = consumerName });
    }

    public async Task StopConsumerAsync(string consumerName)
    {
        await _producer.ProduceAsync(new StopConsumerByName { ConsumerName = consumerName });
    }

    public async Task RestartConsumerAsync(string consumerName)
    {
        await _producer.ProduceAsync(new RestartConsumerByName { ConsumerName = consumerName });
    }

    public async Task ResetOffsetsAsync(string consumerName, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new ResetConsumerOffset
            {
                ConsumerName = consumerName,
                Topics = topics.ToList(),
            });
    }

    public async Task RewindOffsetsAsync(string consumerName, DateTime pointInTime, IEnumerable<string> topics)
    {
        await _producer.ProduceAsync(
            new RewindConsumerOffsetToDateTime
            {
                ConsumerName = consumerName,
                DateTime = pointInTime,
                Topics = topics.ToList(),
            });
    }

    public async Task ChangeWorkersCountAsync(string consumerName, int workersCount)
    {
        await _producer.ProduceAsync(
            new ChangeConsumerWorkersCount
            {
                ConsumerName = consumerName,
                WorkersCount = workersCount,
            });
    }
}
