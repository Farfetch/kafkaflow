namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IConsumer consumer;
        private readonly IDependencyResolverScope workerDependencyResolverScope;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;
        private readonly EventHub eventHub;
        private readonly Channel<IMessageContext> messagesBuffer;

        private readonly Event workerStoppingSubject;
        private readonly Event workerStoppedSubject;

        private CancellationTokenSource stopCancellationTokenSource;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;

        public ConsumerWorker(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            int workerId,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler,
            EventHub eventHub)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.workerDependencyResolverScope = consumerDependencyResolver.CreateScope();
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.eventHub = eventHub;
            this.messagesBuffer = Channel.CreateBounded<IMessageContext>(consumer.Configuration.BufferSize);

            //this.messageConsumeStartEvent = new Event<IMessageContext>(logHandler);

            this.workerStoppingSubject = new(logHandler);
            this.workerStoppedSubject = new(logHandler);

            var middlewareContext = this.workerDependencyResolverScope.Resolver.Resolve<ConsumerMiddlewareContext>();

            middlewareContext.Worker = this;
            middlewareContext.Consumer = consumer;
        }

        public int Id { get; }

        public CancellationToken StopCancellationToken => this.stopCancellationTokenSource?.Token ?? default;

        public IDependencyResolver WorkerDependencyResolver => this.workerDependencyResolverScope.Resolver;

        public IEvent WorkerStopping => this.workerStoppingSubject;

        public IEvent WorkerStopped => this.workerStoppedSubject;

        public ValueTask EnqueueAsync(
            IMessageContext context,
            CancellationToken stopCancellationToken)
        {
            return this.messagesBuffer.Writer.WriteAsync(context, stopCancellationToken);
        }

        public Task StartAsync()
        {
            this.stopCancellationTokenSource = new CancellationTokenSource();

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
            await this.workerStoppingSubject.FireAsync();

            this.messagesBuffer.Writer.TryComplete();

            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.CancelAfter(this.consumer.Configuration.WorkerStopTimeout);
            }

            await this.backgroundTask.ConfigureAwait(false);

            await this.workerStoppedSubject.FireAsync();
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
                await this.eventHub.FireMessageConsumeStartedAsync(context, null);

                try
                {
                    await this.middlewareExecutor
                        .Execute(context, _ => Task.CompletedTask)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    //context.ConsumerContext.ShouldStoreOffset = false;
                }
                catch (Exception ex)
                {
                    //await this.workerError.FireAsync(ex);

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
                finally
                {
                    if (context.ConsumerContext.AutoMessageCompletion)
                    {
                        context.ConsumerContext.Complete();
                    }

                    this.onMessageFinishedHandler?.Invoke();
                }
            }
            catch (Exception ex)
            {
                this.logHandler.Error("KafkaFlow internal message error", ex, null);
            }
        }
    }
}
