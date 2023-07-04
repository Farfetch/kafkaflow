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
        private readonly IDependencyResolver consumerDependencyResolver;
        private readonly IOffsetManager offsetManager;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;
        private readonly GlobalEvents globalEvents;

        private readonly Channel<ConsumeResult<byte[], byte[]>> messagesBuffer;

        private CancellationTokenSource stopCancellationTokenSource;
        private IDependencyResolverScope workerDependencyResolverScope;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            int workerId,
            IOffsetManager offsetManager,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.consumerDependencyResolver = consumerDependencyResolver;
            this.offsetManager = offsetManager;
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.messagesBuffer = Channel.CreateBounded<ConsumeResult<byte[], byte[]>>(consumer.Configuration.BufferSize);
            this.globalEvents = this.dependencyResolver.Resolve<GlobalEvents>();
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
            this.workerDependencyResolverScope = this.consumerDependencyResolver.CreateScope();

            this.backgroundTask = Task.Run(
                async () =>
                {
                    try
                    {
                        var cancellationTokenSource = new CancellationTokenSource();

                        this.stopCancellationTokenSource.Token.Register(
                            () => cancellationTokenSource.CancelAfter(this.consumer.Configuration.WorkerStopTimeout));

                        try
                        {
                            while (await this.messagesBuffer.Reader.WaitToReadAsync(CancellationToken.None).ConfigureAwait(false))
                            {
                                while (this.messagesBuffer.Reader.TryRead(out var message))
                                {
                                    await this.ProcessMessageAsync(message, cancellationTokenSource.Token).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Ignores the exception
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logHandler.Error("KafkaFlow consumer worker fatal error", ex, null);
                    }
                });

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            this.messagesBuffer.Writer.TryComplete();

            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.Cancel();
                this.stopCancellationTokenSource.Dispose();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
            this.workerDependencyResolverScope.Dispose();
        }

        public void OnTaskCompleted(Action handler)
        {
            this.onMessageFinishedHandler = handler;
        }

        private async Task ProcessMessageAsync(ConsumeResult<byte[], byte[]> message, CancellationToken cancellationToken)
        {
            try
            {
                var messageScope = this.consumerDependencyResolver.CreateScope();

                var context = new MessageContext(
                    new Message(message.Message.Key, message.Message.Value),
                    new MessageHeaders(message.Message.Headers),
                    messageScope.Resolver,
                    new ConsumerContext(
                        this.consumer,
                        this.offsetManager,
                        message,
                        cancellationToken,
                        this.Id,
                        this.workerDependencyResolverScope.Resolver,
                        this.consumerDependencyResolver),
                    null);

                try
                {
                    this.offsetManager.OnOffsetProcessed(
                        message.TopicPartitionOffset,
                        () => messageScope.Dispose());

                    await this.globalEvents.FireMessageConsumeStartedAsync(new MessageEventContext(context));

                    await this.middlewareExecutor
                        .Execute(context, _ => Task.CompletedTask)
                        .ConfigureAwait(false);

                    await this.globalEvents.FireMessageConsumeCompletedAsync(new MessageEventContext(context));
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    await this.globalEvents.FireMessageConsumeErrorAsync(new MessageErrorEventContext(context, ex));

                    this.logHandler.Error(
                        "Error processing message",
                        ex,
                        new
                        {
                            context.Message,
                            context.ConsumerContext.Topic,
                            MessageKey = context.Message.Key,
                            context.ConsumerContext.ConsumerName,
                        });
                }

                if (this.consumer.Configuration.AutoStoreOffsets && context.ConsumerContext.ShouldStoreOffset)
                {
                    this.offsetManager.MarkAsProcessed(message.TopicPartitionOffset);
                }

                this.onMessageFinishedHandler?.Invoke();
            }
            catch (Exception ex)
            {
                this.logHandler.Error("KafkaFlow internal message error", ex, null);
            }
        }
    }
}
