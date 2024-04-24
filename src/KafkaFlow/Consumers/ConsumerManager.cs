using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers;

internal class ConsumerManager : IConsumerManager
{
    private readonly IDependencyResolver _dependencyResolver;
    private readonly ILogHandler _logHandler;

    private Timer _evaluateWorkersCountTimer;

    public ConsumerManager(
        IConsumer consumer,
        IConsumerWorkerPool consumerWorkerPool,
        IWorkerPoolFeeder feeder,
        IDependencyResolver dependencyResolver,
        ILogHandler logHandler)
    {
        _dependencyResolver = dependencyResolver;
        _logHandler = logHandler;
        this.Consumer = consumer;
        this.WorkerPool = consumerWorkerPool;
        this.Feeder = feeder;

        this.Consumer.OnPartitionsAssigned((_, _, partitions) => this.OnPartitionAssigned(partitions));
        this.Consumer.OnPartitionsRevoked((_, _, partitions) => this.OnPartitionRevoked(partitions));
    }

    public IWorkerPoolFeeder Feeder { get; }

    public IConsumerWorkerPool WorkerPool { get; }

    public IConsumer Consumer { get; }

    public Task StartAsync()
    {
        this.Feeder.Start();

        _evaluateWorkersCountTimer = new Timer(
            state => _ = this.EvaluateWorkersCountAsync(),
            null,
            this.Consumer.Configuration.WorkersCountEvaluationInterval,
            this.Consumer.Configuration.WorkersCountEvaluationInterval);

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        this.StopEvaluateWorkerCountTimer();

        await this.Feeder.StopAsync().ConfigureAwait(false);
        await this.WorkerPool.StopAsync().ConfigureAwait(false);

        _evaluateWorkersCountTimer?.Dispose();
        _evaluateWorkersCountTimer = null;
        this.Consumer.Dispose();
    }

    private void StopEvaluateWorkerCountTimer() => _evaluateWorkersCountTimer?.Change(Timeout.Infinite, Timeout.Infinite);

    private void StartEvaluateWorkerCountTimer() => _evaluateWorkersCountTimer?.Change(
        this.Consumer.Configuration.WorkersCountEvaluationInterval,
        this.Consumer.Configuration.WorkersCountEvaluationInterval);

    private async Task EvaluateWorkersCountAsync()
    {
        var newWorkersCount = await this.CalculateWorkersCount(this.Consumer.Assignment).ConfigureAwait(false);

        if (newWorkersCount == this.WorkerPool.CurrentWorkersCount)
        {
            return;
        }

        await this.ChangeWorkersCountAsync(newWorkersCount).ConfigureAwait(false);
    }

    private async Task ChangeWorkersCountAsync(int workersCount)
    {
        try
        {
            this.StopEvaluateWorkerCountTimer();

            await this.Feeder.StopAsync().ConfigureAwait(false);
            await this.WorkerPool.StopAsync().ConfigureAwait(false);

            await this.WorkerPool.StartAsync(this.Consumer.Assignment, workersCount).ConfigureAwait(false);
            this.Feeder.Start();

            this.StartEvaluateWorkerCountTimer();
        }
        catch (Exception e)
        {
            _logHandler.Error("Error changing workers count", e, null);
        }
    }

    private void OnPartitionRevoked(IEnumerable<Confluent.Kafka.TopicPartitionOffset> topicPartitions)
    {
        _logHandler.Warning(
            "Partitions revoked",
            this.GetConsumerLogInfo(topicPartitions.Select(x => x.TopicPartition)));

        this.WorkerPool.StopAsync().GetAwaiter().GetResult();
    }

    private void OnPartitionAssigned(IReadOnlyCollection<Confluent.Kafka.TopicPartition> partitions)
    {
        _logHandler.Info(
            "Partitions assigned",
            this.GetConsumerLogInfo(partitions));

        var workersCount = this.CalculateWorkersCount(partitions).GetAwaiter().GetResult();

        this.WorkerPool
            .StartAsync(partitions, workersCount)
            .GetAwaiter()
            .GetResult();
    }

    private object GetConsumerLogInfo(IEnumerable<Confluent.Kafka.TopicPartition> partitions) => new
    {
        this.Consumer.Configuration.GroupId,
        this.Consumer.Configuration.ConsumerName,
        Topics = partitions
            .GroupBy(x => x.Topic)
            .Select(
                x => new
                {
                    x.First().Topic,
                    PartitionsCount = x.Count(),
                    Partitions = x.Select(y => y.Partition.Value),
                }),
    };

    private async Task<int> CalculateWorkersCount(IEnumerable<Confluent.Kafka.TopicPartition> partitions)
    {
        try
        {
            return await this.Consumer.Configuration.WorkersCountCalculator(
                new WorkersCountContext(
                    this.Consumer.Configuration.ConsumerName,
                    this.Consumer.Configuration.GroupId,
                    partitions
                        .GroupBy(
                            x => x.Topic,
                            (topic, grouped) => new TopicPartitions(
                                topic,
                                grouped
                                    .Select(x => x.Partition.Value)
                                    .ToList()))
                        .ToList()),
                _dependencyResolver)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logHandler.Error("Error calculating new workers count, using one worker as fallback", e, null);

            return 1;
        }
    }
}
