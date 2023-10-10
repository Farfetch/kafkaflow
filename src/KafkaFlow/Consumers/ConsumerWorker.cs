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
        private readonly GlobalEvents globalEvents;
        private readonly Channel<IMessageContext> messagesBuffer;

        private readonly Event workerStoppingEvent;
        private readonly Event workerStoppedEvent;
        private readonly Event<IMessageContext> workerProcessingEnded;

        private CancellationTokenSource stopCancellationTokenSource;
        private Task backgroundTask;

        public ConsumerWorker(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            int workerId,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler)
        {
            this.Id = workerId;
            this.consumer = consumer;
            this.workerDependencyResolverScope = consumerDependencyResolver.CreateScope();
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.messagesBuffer = Channel.CreateBounded<IMessageContext>(consumer.Configuration.BufferSize);

            var middlewareContext = this.workerDependencyResolverScope.Resolver.Resolve<ConsumerMiddlewareContext>();
            this.globalEvents = this.workerDependencyResolverScope.Resolver.Resolve<GlobalEvents>();

            middlewareContext.Worker = this;
            middlewareContext.Consumer = consumer;
            this.workerStoppingEvent = new Event(logHandler);
            this.workerStoppedEvent = new Event(logHandler);
            this.workerProcessingEnded = new Event<IMessageContext>(logHandler);
        }

        public int Id { get; }

        public CancellationToken StopCancellationToken => this.stopCancellationTokenSource?.Token ?? default;

        public IDependencyResolver WorkerDependencyResolver => this.workerDependencyResolverScope.Resolver;

        public IEvent WorkerStopping => this.workerStoppingEvent;

        public IEvent WorkerStopped => this.workerStoppedEvent;

        public IEvent<IMessageContext> WorkerProcessingEnded => this.workerProcessingEnded;

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
            await this.workerStoppingEvent.FireAsync();

            this.messagesBuffer.Writer.TryComplete();

            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.CancelAfter(this.consumer.Configuration.WorkerStopTimeout);
            }

            await this.backgroundTask.ConfigureAwait(false);

            await this.workerStoppedEvent.FireAsync();
        }

        public void Dispose()
        {
            this.backgroundTask.Dispose();
            this.workerDependencyResolverScope.Dispose();
            this.stopCancellationTokenSource.Dispose();
        }

        private async Task ProcessMessageAsync(IMessageContext context, CancellationToken cancellationToken)
        {
            try
            {
                await this.globalEvents.FireMessageConsumeStartedAsync(
                    new MessageEventContext(context));

                try
                {
                    await this.middlewareExecutor
                        .Execute(context, _ => Task.CompletedTask)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    context.ConsumerContext.ShouldStoreOffset = false;
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
                finally
                {
                    if (context.ConsumerContext.AutoMessageCompletion)
                    {
                        context.ConsumerContext.Complete(context);
                    }

                    await this.workerProcessingEnded.FireAsync(context);
                }
            }
            catch (Exception ex)
            {
                this.logHandler.Error("KafkaFlow internal message error", ex, null);
            }
        }
    }
}
