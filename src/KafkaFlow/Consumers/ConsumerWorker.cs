namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IConsumer consumer;
        private readonly IDependencyResolver consumerDependencyResolver;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;

        private readonly Channel<IMessageContext> messagesBuffer;

        private CancellationTokenSource stopCancellationTokenSource;
        private IDependencyResolverScope workerDependencyResolverScope;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            int workerId,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.consumerDependencyResolver = consumerDependencyResolver;
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.messagesBuffer = Channel.CreateBounded<IMessageContext>(consumer.Configuration.BufferSize);
        }

        public int Id { get; }

        public CancellationToken StopCancellationToken => this.stopCancellationTokenSource?.Token ?? default;

        public IDependencyResolver WorkerDependencyResolver => this.workerDependencyResolverScope.Resolver;

        public ValueTask EnqueueAsync(
            IMessageContext context,
            CancellationToken stopCancellationToken)
        {
            return this.messagesBuffer.Writer.WriteAsync(context, stopCancellationToken);
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
                        try
                        {
                            while (await this.messagesBuffer.Reader.WaitToReadAsync(CancellationToken.None).ConfigureAwait(false))
                            {
                                while (this.messagesBuffer.Reader.TryRead(out var message))
                                {
                                    await this.ProcessMessageAsync(message, this.stopCancellationTokenSource.Token).ConfigureAwait(false);
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
                },
                CancellationToken.None);

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            this.messagesBuffer.Writer.TryComplete();

            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.CancelAfter(this.consumer.Configuration.WorkerStopTimeout);
            }

            await this.backgroundTask.ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.backgroundTask.Dispose();
            this.workerDependencyResolverScope.Dispose();
            this.stopCancellationTokenSource.Dispose();
        }

        public void OnTaskCompleted(Action handler)
        {
            this.onMessageFinishedHandler = handler;
        }

        private async Task ProcessMessageAsync(IMessageContext context, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    await this.middlewareExecutor
                        .Execute(context, _ => Task.CompletedTask)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
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
                    context.ConsumerContext.StoreOffset();
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
