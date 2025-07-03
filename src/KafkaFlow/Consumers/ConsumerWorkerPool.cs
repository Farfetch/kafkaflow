using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Configuration;
using KafkaFlow.Extensions;

namespace KafkaFlow.Consumers;

internal class ConsumerWorkerPool : IConsumerWorkerPool
{
    private readonly IConsumer _consumer;
    private readonly IDependencyResolver _consumerDependencyResolver;
    private readonly IMiddlewareExecutor _middlewareExecutor;
    private readonly ILogHandler _logHandler;
    private readonly Factory<IWorkerDistributionStrategy> _distributionStrategyFactory;
    private readonly IOffsetCommitter _offsetCommitter;

    private readonly Event _workerPoolStoppedSubject;

    private TaskCompletionSource<object> _startedTaskSource = new();
    private List<IConsumerWorker> _workers = new();

    private CancellationTokenSource _stopCancellationTokenSource;
    private IWorkerDistributionStrategy _distributionStrategy;
    private IOffsetManager _offsetManager;

    public ConsumerWorkerPool(
        IConsumer consumer,
        IDependencyResolver consumerDependencyResolver,
        IMiddlewareExecutor middlewareExecutor,
        IConsumerConfiguration consumerConfiguration,
        ILogHandler logHandler)
    {
        _consumer = consumer;
        _consumerDependencyResolver = consumerDependencyResolver;
        _middlewareExecutor = middlewareExecutor;
        _logHandler = logHandler;
        _distributionStrategyFactory = consumerConfiguration.DistributionStrategyFactory;
        _workerPoolStoppedSubject = new Event(logHandler);

        _offsetCommitter = consumer.Configuration.NoStoreOffsets ?
            new NullOffsetCommitter() :
            new OffsetCommitter(
                consumer,
                consumerDependencyResolver,
                logHandler);

        _offsetCommitter.PendingOffsetsStatisticsHandlers.AddRange(consumer.Configuration.PendingOffsetsStatisticsHandlers);
    }

    public int CurrentWorkersCount { get; private set; }

    public IEvent WorkerPoolStopped => _workerPoolStoppedSubject;

    public async Task StartAsync(IReadOnlyCollection<Confluent.Kafka.TopicPartition> partitions, int workersCount)
    {
        try
        {
            _offsetManager = _consumer.Configuration.NoStoreOffsets ?
                new NullOffsetManager() :
                new OffsetManager(_offsetCommitter, partitions);

            await _offsetCommitter.StartAsync().ConfigureAwait(false);

            this.CurrentWorkersCount = workersCount;

            _stopCancellationTokenSource = new CancellationTokenSource();

            var subscription = _consumer.MaxPollIntervalExceeded.Subscribe(
                () =>
                {
                    _stopCancellationTokenSource.Cancel();
                    return Task.CompletedTask;
                });

            _stopCancellationTokenSource.Token.Register(() => subscription.Cancel());

            await Task.WhenAll(
                Enumerable
                    .Range(0, this.CurrentWorkersCount)
                    .Select(
                        workerId =>
                        {
                            var worker = new ConsumerWorker(
                                _consumer,
                                _consumerDependencyResolver,
                                workerId,
                                _middlewareExecutor,
                                _logHandler);

                            _workers.Add(worker);

                            return worker.StartAsync(_stopCancellationTokenSource.Token);
                        }))
                .ConfigureAwait(false);

            _distributionStrategy = _distributionStrategyFactory(_consumerDependencyResolver);
            _distributionStrategy.Initialize(_workers.AsReadOnly());

            _startedTaskSource.TrySetResult(null);
        }
        catch (Exception e)
        {
            _logHandler.Error(
                "Error starting WorkerPool",
                e,
                new
                {
                    _consumer.Configuration.ConsumerName,
                });
        }
    }

    public async Task StopAsync()
    {
        if (_workers.Count == 0)
        {
            return;
        }

        var currentWorkers = _workers;
        _workers = new List<IConsumerWorker>();
        _startedTaskSource = new();

        if (_stopCancellationTokenSource.Token.CanBeCanceled)
        {
            _stopCancellationTokenSource.CancelAfter(_consumer.Configuration.WorkerStopTimeout);
        }

        await Task.WhenAll(currentWorkers.Select(x => x.StopAsync())).ConfigureAwait(false);
        await _offsetManager
            .WaitContextsCompletionAsync()
            .WithCancellation(_stopCancellationTokenSource.Token, false)
            .ConfigureAwait(false);

        currentWorkers.ForEach(worker => worker.Dispose());
        _stopCancellationTokenSource?.Dispose();

        _offsetManager = null;

        await _workerPoolStoppedSubject.FireAsync().ConfigureAwait(false);

        await _offsetCommitter.StopAsync().ConfigureAwait(false);
    }

    public async Task EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken)
    {
        await _startedTaskSource.Task.ConfigureAwait(false);

        var worker = (IConsumerWorker)await _distributionStrategy
            .GetWorkerAsync(
                new WorkerDistributionContext(
                    _consumer.Configuration.ConsumerName,
                    message.Topic,
                    message.Partition.Value,
                    message.Message.Key,
                    stopCancellationToken)).ConfigureAwait(false);

        if (worker is null)
        {
            return;
        }

        var context = this.CreateMessageContext(message, worker);

        _offsetManager.Enqueue(context.ConsumerContext);

        await worker.EnqueueAsync(context).ConfigureAwait(false);
    }

    private MessageContext CreateMessageContext(ConsumeResult<byte[], byte[]> message, IConsumerWorker worker)
    {
        var messageDependencyScope = _consumerDependencyResolver.CreateScope();

        var context = new MessageContext(
            new Message(message.Message.Key, message.Message.Value),
            new MessageHeaders(message.Message.Headers),
            messageDependencyScope.Resolver,
            new ConsumerContext(
                _consumer,
                _offsetManager,
                message,
                worker,
                messageDependencyScope,
                _consumerDependencyResolver),
            null,
            _consumer.Configuration.ClusterConfiguration.Brokers);
        return context;
    }
}
