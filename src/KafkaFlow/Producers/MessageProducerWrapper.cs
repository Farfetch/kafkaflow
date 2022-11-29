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
            object messageKey,
            object message,
            IMessageHeaders headers = null,
            int? partition = null)
        {
            return this.producer.ProduceAsync(
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
            return this.producer.ProduceAsync(
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
            this.producer.Produce(
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
            this.producer.Produce(
                messageKey,
                message,
                headers,
                deliveryHandler,
                partition);
        }
    }
}
