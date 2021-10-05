namespace KafkaFlow.Producers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class MessageProducerWrapper<TProducer> : IMessageProducer<TProducer>
    {
        private readonly IMessageProducer producer;

        public MessageProducerWrapper(IMessageProducer producer)
        {
            this.producer = producer;
        }

        public string ProducerName => this.producer.ProducerName;

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string topic,
            object messageKey,
            object message,
            IMessageHeaders headers = null,
            CancellationToken cancellationToken = default)
        {
            return this.producer.ProduceAsync(
                topic,
                messageKey,
                message,
                headers,
                cancellationToken);
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            object partitionKey,
            object message,
            IMessageHeaders headers = null,
            CancellationToken cancellationToken = default)
        {
            return this.producer.ProduceAsync(
                partitionKey,
                message,
                headers,
                cancellationToken);
        }

        public void Produce(
            string topic,
            object partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
            CancellationToken cancellationToken = default)
        {
            this.producer.Produce(
                topic,
                partitionKey,
                message,
                headers,
                deliveryHandler,
                cancellationToken);
        }

        public void Produce(
            object partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
            CancellationToken cancellationToken = default)
        {
            this.producer.Produce(
                partitionKey,
                message,
                headers,
                deliveryHandler,
                cancellationToken);
        }
    }
}
