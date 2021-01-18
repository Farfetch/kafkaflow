namespace KafkaFlow.Producers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class MessageProducer : IMessageProducer, IDisposable
    {
        private readonly ProducerConfiguration configuration;
        private readonly MiddlewareExecutor middlewareExecutor;
        private readonly IDependencyResolverScope dependencyResolverScope;

        private volatile IProducer<byte[], byte[]> producer;
        private readonly object producerCreationSync = new object();

        public MessageProducer(
            IDependencyResolver dependencyResolver,
            ProducerConfiguration configuration)
        {
            this.configuration = configuration;

            // Create middlewares instances inside a scope to allow scoped injections in producer middlewares
            this.dependencyResolverScope = dependencyResolver.CreateScope();

            var middlewares = this.configuration.MiddlewareConfiguration.Factories
                .Select(factory => factory(this.dependencyResolverScope.Resolver))
                .ToList();

            this.middlewareExecutor = new MiddlewareExecutor(middlewares);
        }

        public string ProducerName => this.configuration.Name;

        public async Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            var messageKey = partitionKey is null ? null : Encoding.UTF8.GetBytes(partitionKey);

            DeliveryResult<byte[], byte[]> report = null;

            try
            {
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
            }
            catch (Exception ex) when (!(ex is ProduceException<byte[], byte[]> || ex is KafkaException))
            {
                this.dependencyResolverScope.Resolver
                    .Resolve<ILogHandler>()
                    .Error("Error executing producer",
                        ex,
                        new
                        {
                            message,
                            topic,
                            partitionKey
                        });
                throw;
            }

            return report;
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            if (string.IsNullOrWhiteSpace(this.configuration.DefaultTopic))
            {
                throw new InvalidOperationException(
                    $"There is no default topic defined for producer {this.ProducerName}");
            }

            return this.ProduceAsync(
                this.configuration.DefaultTopic,
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
            var messageKey = partitionKey is null ? null : Encoding.UTF8.GetBytes(partitionKey);

            try
            {
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
                                completionSource.SetException(new ProduceException<byte[], byte[]>(report.Error, report));
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
            catch (Exception ex) when (!(ex is ProduceException<byte[], byte[]> || ex is KafkaException))
            {
                this.dependencyResolverScope.Resolver
                    .Resolve<ILogHandler>()
                    .Error("Error executing producer",
                        ex,
                        new
                        {
                            message,
                            topic,
                            partitionKey
                        });
                throw;
            }
        }

        public void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            if (string.IsNullOrWhiteSpace(this.configuration.DefaultTopic))
            {
                throw new InvalidOperationException(
                    $"There is no default topic defined for producer {this.ProducerName}");
            }

            this.Produce(
                this.configuration.DefaultTopic,
                partitionKey,
                message,
                headers,
                deliveryHandler);
        }

        private IProducer<byte[], byte[]> EnsureProducer()
        {
            if (this.producer != null)
            {
                return this.producer;
            }

            lock (this.producerCreationSync)
            {
                if (this.producer != null)
                {
                    return this.producer;
                }

                return this.producer = new ProducerBuilder<byte[], byte[]>(this.configuration.GetKafkaConfig())
                    .SetErrorHandler(
                        (p, error) =>
                        {
                            if (error.IsFatal)
                            {
                                this.InvalidateProducer(error, null);
                            }
                            else
                            {
                                this.dependencyResolverScope.Resolver
                                    .Resolve<ILogHandler>()
                                    .Warning("Kafka Producer Error", new { Error = error });
                            }
                        })
                    .SetStatisticsHandler((producer, statistics) =>
                    {
                        foreach (var handler in this.configuration.StatisticsHandlers)
                        {
                            handler.Invoke(statistics);
                        }
                    })
                    .Build();
            }
        }

        private void InvalidateProducer(Error error, DeliveryResult<byte[], byte[]> result)
        {
            lock (this.producerCreationSync)
            {
                this.producer = null;
            }

            this.dependencyResolverScope.Resolver
                .Resolve<ILogHandler>()
                .Error(
                    "Kafka produce fatal error occurred. The producer will be recreated",
                    result is null ? new KafkaException(error) : new ProduceException<byte[], byte[]>(error, result),
                    new { Error = error });
        }

        private async Task<DeliveryResult<byte[], byte[]>> InternalProduceAsync(ProducerMessageContext context)
        {
            DeliveryResult<byte[], byte[]> result = null;

            try
            {
                result = await this
                    .EnsureProducer()
                    .ProduceAsync(
                        context.Topic,
                        CreateMessage(context))
                    .ConfigureAwait(false);
            }
            catch (ProduceException<byte[], byte[]> e)
            {
                if (e.Error.IsFatal)
                {
                    this.InvalidateProducer(e.Error, result);
                }

                throw;
            }

            context.Offset = result.Offset;
            context.Partition = result.Partition;

            return result;
        }

        private void InternalProduce(
            ProducerMessageContext context,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler)
        {
            this
                .EnsureProducer()
                .Produce(
                    context.Topic,
                    CreateMessage(context),
                    report =>
                    {
                        if (report.Error.IsFatal)
                        {
                            this.InvalidateProducer(report.Error, report);
                        }

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

        public void Dispose()
        {
            this.dependencyResolverScope.Dispose();
            this.producer?.Dispose();
        }
    }
}
