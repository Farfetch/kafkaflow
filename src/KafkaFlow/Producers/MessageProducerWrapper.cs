using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

internal class MessageProducerWrapper<TProducer> : IMessageProducer<TProducer>
{
    private readonly IMessageProducer _producer;

    public MessageProducerWrapper(IMessageProducer producer)
    {
        _producer = producer;
    }

    public string ProducerName => _producer.ProducerName;

    public Task<IReadOnlyCollection<BatchProduceItem>> BatchProduceAsync(
        IReadOnlyCollection<BatchProduceItem> items,
        bool throwIfAnyProduceFail = true)
    {
        return _producer.BatchProduceAsync(items, throwIfAnyProduceFail);
    }

    public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
        string topic,
        object messageKey,
        object message,
        IMessageHeaders headers = null,
        int? partition = null)
    {
        return _producer.ProduceAsync(
            topic,
            messageKey,
            message,
            headers,
            partition);
    }

    public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
        object messageKey,
        object message,
        IMessageHeaders headers = null,
        int? partition = null)
    {
        return _producer.ProduceAsync(
            messageKey,
            message,
            headers,
            partition);
    }

    public void Produce(
        string topic,
        object messageKey,
        object message,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        _producer.Produce(
            topic,
            messageKey,
            message,
            headers,
            deliveryHandler,
            partition);
    }

    public void Produce(
        object messageKey,
        object message,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        _producer.Produce(
            messageKey,
            message,
            headers,
            deliveryHandler,
            partition);
    }
}
