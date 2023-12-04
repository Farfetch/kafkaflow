using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers
{
    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly IConsumer _consumer;
        private readonly IDependencyResolverScope _workerDependencyResolverScope;
        private readonly IMiddlewareExecutor _middlewareExecutor;
        private readonly ILogHandler _logHandler;
        private readonly GlobalEvents _globalEvents;

        private readonly Channel<IMessageContext> _messagesBuffer;

        private readonly Event _workerStoppingEvent;
        private readonly Event _workerStoppedEvent;
        private readonly Event<IMessageContext> _workerProcessingEnded;

        private CancellationTokenSource _stopCancellationTokenSource;
        private Task _backgroundTask;

        public ConsumerWorker(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            int workerId,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler)
        {
            this.Id = workerId;
            _consumer = consumer;
            _workerDependencyResolverScope = consumerDependencyResolver.CreateScope();
            _middlewareExecutor = middlewareExecutor;
            _logHandler = logHandler;
            _messagesBuffer = Channel.CreateBounded<IMessageContext>(consumer.Configuration.BufferSize);
            _globalEvents = consumerDependencyResolver.Resolve<GlobalEvents>();

            _workerStoppingEvent = new(logHandler);
            _workerStoppedEvent = new(logHandler);
            _workerProcessingEnded = new Event<IMessageContext>(logHandler);

            var middlewareContext = _workerDependencyResolverScope.Resolver.Resolve<ConsumerMiddlewareContext>();

            middlewareContext.Worker = this;
            middlewareContext.Consumer = consumer;
        }

        public int Id { get; }

        public CancellationToken StopCancellationToken => _stopCancellationTokenSource?.Token ?? default;

        public IDependencyResolver WorkerDependencyResolver => _workerDependencyResolverScope.Resolver;

        public IEvent WorkerStopping => _workerStoppingEvent;

        public IEvent WorkerStopped => _workerStoppedEvent;

        public IEvent<IMessageContext> WorkerProcessingEnded => _workerProcessingEnded;

        public ValueTask EnqueueAsync(IMessageContext context)
        {
            return _messagesBuffer.Writer.WriteAsync(context, CancellationToken.None);
        }

        public Task StartAsync()
        {
            _stopCancellationTokenSource = new CancellationTokenSource();

            _backgroundTask = Task.Run(
                async () =>
                {
                    try
                    {
                        try
                        {
                            while (await _messagesBuffer.Reader.WaitToReadAsync(CancellationToken.None).ConfigureAwait(false))
                            {
                                while (_messagesBuffer.Reader.TryRead(out var message))
                                {
                                    await this.ProcessMessageAsync(message, _stopCancellationTokenSource.Token).ConfigureAwait(false);
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
                        _logHandler.Error("KafkaFlow consumer worker fatal error", ex, null);
                    }
                },
                CancellationToken.None);

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await _workerStoppingEvent.FireAsync();

            _messagesBuffer.Writer.TryComplete();

            if (_stopCancellationTokenSource.Token.CanBeCanceled)
            {
                _stopCancellationTokenSource.CancelAfter(_consumer.Configuration.WorkerStopTimeout);
            }

            await _backgroundTask.ConfigureAwait(false);

            await _workerStoppedEvent.FireAsync();
        }

        public void Dispose()
        {
            _backgroundTask.Dispose();
            _workerDependencyResolverScope.Dispose();
            _stopCancellationTokenSource.Dispose();
        }

        private async Task ProcessMessageAsync(IMessageContext context, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    await _globalEvents.FireMessageConsumeStartedAsync(new MessageEventContext(context));

                    _ = context.ConsumerContext.Completion.ContinueWith(
                        async task =>
                        {
                            if (task.IsFaulted)
                            {
                                await _globalEvents.FireMessageConsumeErrorAsync(new MessageErrorEventContext(context, task.Exception));
                            }

                            await _globalEvents.FireMessageConsumeCompletedAsync(new MessageEventContext(context));
                        });

                    await _middlewareExecutor
                        .Execute(context, _ => Task.CompletedTask)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    context.ConsumerContext.ShouldStoreOffset = false;
                }
                catch (Exception ex)
                {
                    await _globalEvents.FireMessageConsumeErrorAsync(new MessageErrorEventContext(context, ex));

                    _logHandler.Error(
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

                    await _workerProcessingEnded.FireAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logHandler.Error("KafkaFlow internal message error", ex, null);
            }
        }
    }
}
