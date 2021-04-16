namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource stopCancellationTokenSource;

        private readonly Channel<ConsumeResult<byte[], byte[]>> messagesBuffer;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IConsumer consumer,
            int workerId,
            IOffsetManager offsetManager,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<ConsumeResult<byte[], byte[]>>(consumer.Configuration.BufferSize);
        }

        public int Id { get; }

        public ValueTask EnqueueAsync(
            ConsumeResult<byte[], byte[]> message,
            CancellationToken stopCancellationToken = default)
        {
            return this.messagesBuffer.Writer.WriteAsync(message, stopCancellationToken);
        }

        public Task StartAsync()
        {
            this.stopCancellationTokenSource = new CancellationTokenSource();

            this.backgroundTask = Task.Run(
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
                                    this.offsetManager,
                                    message,
                                    this.stopCancellationTokenSource.Token),
                                message,
                                this.Id,
                                this.consumer.Configuration.GroupId);

                            try
                            {
                                await this.middlewareExecutor
                                    .Execute(context, _ => Task.CompletedTask)
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
                                if (this.consumer.Configuration.AutoStoreOffsets && context.Consumer.ShouldStoreOffset)
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
                });

            return Task.CompletedTask;
        }

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
