namespace KafkaFlow.Producers
{
    using System;
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
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.producer.ProduceAsync(
                topic,
                partitionKey,
                message,
                headers);
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.producer.ProduceAsync(
                partitionKey,
                message,
                headers);
        }

        public void Produce(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            this.producer.Produce(
                topic,
                partitionKey,
                message,
                headers,
                deliveryHandler);
        }

        public void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            this.producer.Produce(
                partitionKey,
                message,
                headers,
                deliveryHandler);
        }
    }
}
