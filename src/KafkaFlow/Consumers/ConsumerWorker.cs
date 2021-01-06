namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IKafkaConsumer consumer;
        private readonly ConsumerConfiguration configuration;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource stopCancellationTokenSource;

        private readonly Channel<ConsumeResult<byte[], byte[]>> messagesBuffer;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IKafkaConsumer consumer,
            int workerId,
            ConsumerConfiguration configuration,
            IOffsetManager offsetManager,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.configuration = configuration;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<ConsumeResult<byte[], byte[]>>(configuration.BufferSize);
        }

        public int Id { get; }

        public ValueTask EnqueueAsync(
            ConsumeResult<byte[], byte[]> message,
            CancellationToken stopCancellationToken = default)
        {
            return this.messagesBuffer.Writer.WriteAsync(message, stopCancellationToken);
        }

        public Task StartAsync(CancellationToken stopCancellationToken)
        {
            this.stopCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    while (!this.stopCancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await this.messagesBuffer.Reader
                                .ReadAsync(this.stopCancellationTokenSource.Token)
                                .ConfigureAwait(false);

                            var context = new ConsumerMessageContext(
                                new MessageContextConsumer(
                                    this.consumer,
                                    this.configuration.ConsumerName,
                                    this.offsetManager,
                                    message,
                                    this.stopCancellationTokenSource.Token),
                                message,
                                this.Id,
                                this.configuration.GroupId);

                            try
                            {
                                await this.middlewareExecutor
                                    .Execute(context, con => Task.CompletedTask)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Error(
                                    "Error executing consumer",
                                    ex,
                                    new
                                    {
                                        context.Message,
                                        context.Topic,
                                        context.PartitionKey,
                                        ConsumerName = context.Consumer.Name
                                    });
                            }
                            finally
                            {
                                if (this.configuration.AutoStoreOffsets && context.Consumer.ShouldStoreOffset)
                                {
                                    this.offsetManager.StoreOffset(message.TopicPartitionOffset);
                                }

                                this.onMessageFinishedHandler?.Invoke();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Ignores the exception
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public Task StartAsync() => this.StartAsync(default);

        public async Task StopAsync()
        {
            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.Cancel();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        public void OnTaskCompleted(Action handler)
        {
            this.onMessageFinishedHandler = handler;
        }
    }
}
