namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    public class TriggerErrorSerializerMiddleware : ISerializer
    {
        /// <inheritdoc/>
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            var error = new Error(ErrorCode.BrokerNotAvailable);
            throw new ProduceException<byte[], byte[]>(error, null);
        }

        /// <inheritdoc/>
        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            var error = new Error(ErrorCode.BrokerNotAvailable);
            throw new ProduceException<byte[], byte[]>(error, null);
        }
    }
}
