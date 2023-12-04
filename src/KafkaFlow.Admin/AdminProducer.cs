using System;
using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;

namespace KafkaFlow.Admin;

internal class AdminProducer : IAdminProducer
{
    private readonly IMessageProducer<AdminProducer> _producer;
    private readonly int _topicPartition;

    public AdminProducer(IMessageProducer<AdminProducer> producer, int topicPartition)
    {
        _producer = producer;
        _topicPartition = topicPartition;
    }

    public Task ProduceAsync(IAdminMessage message) =>
        _producer.ProduceAsync(Guid.NewGuid().ToString(), message, partition: _topicPartition);
}
