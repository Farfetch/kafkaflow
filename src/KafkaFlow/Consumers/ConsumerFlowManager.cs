using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow.Consumers;

internal class ConsumerFlowManager : IConsumerFlowManager
{
    private readonly IConsumer _consumer;
    private readonly ILogHandler _logHandler;
    private readonly List<TopicPartition> _pausedPartitions = new();
    private readonly SemaphoreSlim _consumerSemaphore = new(1, 1);

    private IConsumer<byte[], byte[]> _clientConsumer;
    private CancellationTokenSource _heartbeatTokenSource;
    private Task _heartbeatTask;

    public ConsumerFlowManager(
        IConsumer consumer,
        ILogHandler logHandler)
    {
        _consumer = consumer;
        _logHandler = logHandler;
    }

    public IReadOnlyList<TopicPartition> PausedPartitions => _pausedPartitions.AsReadOnly();

    public void Pause(IReadOnlyCollection<TopicPartition> topicPartitions)
    {
        lock (_pausedPartitions)
        {
            topicPartitions = topicPartitions.Except(_pausedPartitions).ToList();

            if (!topicPartitions.Any())
            {
                return;
            }

            _clientConsumer.Pause(topicPartitions);
            _pausedPartitions.AddRange(topicPartitions);

            if (_consumer.Status != ConsumerStatus.Paused)
            {
                return;
            }

            this.StartHeartbeat();
        }
    }

    public Task BlockHeartbeat(CancellationToken cancellationToken)
    {
        return _consumerSemaphore.WaitAsync(cancellationToken);
    }

    public void ReleaseHeartbeat()
    {
        if (_consumerSemaphore.CurrentCount != 1)
        {
            _consumerSemaphore.Release();
        }
    }

    public void Resume(IReadOnlyCollection<TopicPartition> topicPartitions)
    {
        lock (_pausedPartitions)
        {
            if (!topicPartitions.Any())
            {
                return;
            }

            foreach (var topicPartition in topicPartitions)
            {
                _pausedPartitions.Remove(topicPartition);
            }

            if (_consumer.Status == ConsumerStatus.Paused)
            {
                return;
            }

            this.StopHeartbeat();

            _clientConsumer.Resume(topicPartitions);
        }
    }

    public void Start(IConsumer<byte[], byte[]> clientConsumer)
    {
        _clientConsumer = clientConsumer;
    }

    public void Stop()
    {
        _pausedPartitions.Clear();
        this.StopHeartbeat();
    }

    private void StartHeartbeat()
    {
        _heartbeatTokenSource = new CancellationTokenSource();

        _heartbeatTask = Task.Run(
            () =>
            {
                if (_consumerSemaphore.Wait(0))
                {
                    try
                    {
                        const int consumeTimeoutCall = 1000;

                        while (!_heartbeatTokenSource.IsCancellationRequested)
                        {
                            var result = _clientConsumer.Consume(consumeTimeoutCall);

                            if (result != null)
                            {
                                _logHandler.Warning(
                                    "Paused consumer heartbeat process wrongly read a message, please report this issue",
                                    null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logHandler.Error(
                            "Error executing paused consumer background heartbeat",
                            ex,
                            null);
                    }
                    finally
                    {
                        this.ReleaseHeartbeat();
                    }
                }
            });
    }

    private void StopHeartbeat()
    {
        _heartbeatTokenSource?.Cancel();
        _heartbeatTask?.GetAwaiter().GetResult();
        _heartbeatTask?.Dispose();
    }
}
