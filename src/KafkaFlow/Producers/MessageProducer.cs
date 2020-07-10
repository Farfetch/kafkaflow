namespace KafkaFlow.Producers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class MessageProducer<TProducer> : IMessageProducer<TProducer>
    {
        private readonly ProducerConfiguration configuration;
        private readonly IProducer<byte[], byte[]> producer;
        private readonly MiddlewareExecutor middlewareExecutor;

        public MessageProducer(
            IDependencyResolver dependencyResolver,
            string producerName,
            ProducerConfiguration configuration)
        {
            this.ProducerName = producerName;
            this.configuration = configuration;

            this.producer = new ProducerBuilder<byte[], byte[]>(configuration.GetKafkaConfig()).Build();

            var middlewares = this.configuration.MiddlewareConfiguration.Factories
                .Select(factory => factory(dependencyResolver))
                .ToList();

            this.middlewareExecutor = new MiddlewareExecutor(middlewares);
        }

        public string ProducerName { get; }

        public async Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            var messageKey = Encoding.UTF8.GetBytes(partitionKey);

            DeliveryResult<byte[], byte[]> report = null;

            await this.middlewareExecutor
                .Execute(
                    new ProducerMessageContext(
                        message,
                        messageKey,
                        headers,
                        topic),
                    async context =>
                    {
                        report = await this
                            .InternalProduceAsync((ProducerMessageContext) context)
                            .ConfigureAwait(false);
                    })
                .ConfigureAwait(false);

            return report;
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.ProduceAsync(
                this.configuration.Topic,
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
            var messageKey = Encoding.UTF8.GetBytes(partitionKey);

            this.middlewareExecutor.Execute(
                new ProducerMessageContext(
                    message,
                    messageKey,
                    headers,
                    topic),
                context =>
                {
                    var completionSource = new TaskCompletionSource<byte>();

                    this.InternalProduce(
                        (ProducerMessageContext) context,
                        report =>
                        {
                            if (report.Error.IsError)
                            {
                                completionSource.SetException(new KafkaException(report.Error));
                            }
                            else
                            {
                                completionSource.SetResult(0);
                            }

                            deliveryHandler?.Invoke(report);
                        });

                    return completionSource.Task;
                });
        }

        public void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            this.Produce(
                this.configuration.Topic,
                partitionKey,
                message,
                headers,
                deliveryHandler);
        }

        private async Task<DeliveryResult<byte[], byte[]>> InternalProduceAsync(ProducerMessageContext context)
        {
            var result = await this.producer
                .ProduceAsync(
                    context.Topic,
                    CreateMessage(context))
                .ConfigureAwait(false);

            context.Offset = result.Offset;
            context.Partition = result.Partition;

            return result;
        }

        private void InternalProduce(
            ProducerMessageContext context,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler)
        {
            this.producer
                .Produce(
                    context.Topic,
                    CreateMessage(context),
                    report =>
                    {
                        context.Offset = report.Offset;
                        context.Partition = report.Partition;

                        deliveryHandler(report);
                    });
        }

        private static Message<byte[], byte[]> CreateMessage(IMessageContext context)
        {
            return new Message<byte[], byte[]>
            {
                Key = context.PartitionKey,
                Value = GetMessageContent(context),
                Headers = ((MessageHeaders) context.Headers).GetKafkaHeaders(),
                Timestamp = Timestamp.Default
            };
        }

        private static byte[] GetMessageContent(IMessageContext context)
        {
            if (!(context.Message is byte[] value))
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be produced, it is a {context.Message.GetType().FullName}." +
                    "You should serialize or encode your message object using a middleware");
            }

            return value;
        }
    }
}
