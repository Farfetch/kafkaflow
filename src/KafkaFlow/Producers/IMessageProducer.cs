namespace KafkaFlow.Producers
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    /// <summary>
    /// Provides access to the kafka message producer
    /// </summary>
    public interface IMessageProducer<TProducer> : IMessageProducer
    {
    }

    /// <summary>
    /// Provides access to the kafka producer
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        /// The unique producer name
        /// </summary>
        string ProducerName { get; }

        /// <summary>
        /// Produces a new message
        /// </summary>
        /// <param name="topic">The topic where the message wil be produced</param>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <returns></returns>
        Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null);

        /// <summary>
        /// Produces a new message in the configured default topic
        /// </summary>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <returns></returns>
        Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null);

        /// <summary>
        /// Produces a new message
        /// This should be used for high throughput scenarios: <see href="https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer#produceasync-vs-produce"/>
        /// </summary>
        /// <param name="topic">The topic where the message wil be produced</param>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <param name="deliveryHandler">A handler with the operation result</param>
        /// <returns></returns>
        void Produce(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null);

        /// <summary>
        /// Produces a new message in the configured default topic
        /// This should be used for high throughput scenarios: <see href="https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer#produceasync-vs-produce"/>
        /// </summary>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <param name="deliveryHandler">A handler with the operation result</param>
        /// <returns></returns>
        void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null);
    }
}
